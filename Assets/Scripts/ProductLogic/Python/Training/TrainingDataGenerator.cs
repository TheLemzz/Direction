using CityGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Class for automaticly generate YOLO annotations.
/// </summary>
public sealed class TrainingDataGenerator : MonoBehaviour
{
    [Header("Настройки:"), Space(10)]
    [SerializeField] private LayerMask _mask;
    [Range(0.5f, 10f), SerializeField] private float minDistance = 3f;
    [SerializeField] private int imageWidth = 1920;
    [SerializeField] private int imageHeight = 1080;
    [SerializeField] private KeyCode captureKey = KeyCode.Space;

    [Header("Автоматизация:"), Space(10)]
    [SerializeField] private bool autoCapture = false;
    [SerializeField, Range(0.2f, 10f), ConditionalBoolField(nameof(autoCapture))] private float time = 3;

    [Header("Дополнительно:"), Space(10)]
    [Tooltip("Изображения не будут делаться в случае, если носитель камеры неподвижен."), SerializeField] private bool preventStaticPhotos = true;
    [SerializeField, ConditionalBoolField(nameof(preventStaticPhotos))] private TrafficCar _car;

    [Header("Camera Randomization Settings")]
    [SerializeField] private bool _useRandomization = true;

    [Tooltip("Максимальный поворот камеры влево/вправо (Yaw) в градусах")]
    [SerializeField] private float maxYawAngle = 10f;

    [Tooltip("Максимальный наклон камеры вверх/вниз (Pitch) в градусах")]
    [SerializeField] private float maxPitchAngle = 6f;

    [Tooltip("Диапазон изменения Field of View")]
    [SerializeField] private Vector2 fovRange = new(40f, 75f);

    private Camera renderCamera;
    private MLManager _mlManager;
    private MLSaver _saver;

    private Quaternion _initialRotation;

    private TrainingData[] _trainingData;
    private readonly Collider[] _results = new Collider[500];
    private readonly Dictionary<string, int> _names = new();

    private int backgroundPercent;
    private int imageQuality;

    private float maxDistance;
    private float _initialFOV;

    private bool _init = false;
    private bool _fog = false;
    private bool renderCameraInit = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    private void OnEnable()
    {
        WeatherManager.OnWeatherChanged += OnFog;
    }

    private void OnDisable()
    {
        WeatherManager.OnWeatherChanged -= OnFog;
    }

    private void OnFog(bool started, WeatherType type)
    {
        if (type == WeatherType.Fog)
        {
            _fog = started;
        }
    }

    public void Init(TrainingData[] trainingData, MLManager mlManager)
    {
        if (_init) return;

        _init = true;
        renderCamera = GetComponent<Camera>();
        _saver = MLSaver.GetInstance();
        BaseSaveData data = _saver.GetCurrentSaveData<BaseSaveData>();

        backgroundPercent = data.BackgroundPercent;
        imageQuality = data.ImageQuality;

        _mlManager = mlManager;
        print(trainingData.Length);
        _trainingData = trainingData;

        InitializeCameraBase();
        Compute();
    }

    private void Compute()
    {
        maxDistance = _trainingData.Max(x => x.Range) - 2;

        time += Random.Range(-0.15f, 0.15f);

        if (autoCapture)
        {
            StartCoroutine(AutoCaptureData());
        }
    }

    private void Update()
    {
        if (autoCapture || !_init) return;

        if (Input.GetKeyDown(captureKey))
        {
            StartCoroutine(CaptureTrainingData());
        }
    }

    private IEnumerator AutoCaptureData()
    {
        WaitForSeconds seconds = new(time);

        while (true)
        {
            yield return seconds;

            if (preventStaticPhotos && _car.GetSpeed() <= 5) continue;

            StartCoroutine(CaptureTrainingData());
        }
    }

    public IEnumerator CaptureTrainingData()
    {
        yield return new WaitForEndOfFrame();

        if (_useRandomization)
        {
            ResetCamera();
            if (Random.Range(0, 100) <= 45) RandomizeCamera();
        }

        List<string> annots = GenerateAnnotations(Random.Range(0, 100) <= backgroundPercent);

        if (annots == null)
        {
            yield break;
        }

        RenderTexture rt = new(imageWidth, imageHeight, 24);
        renderCamera.targetTexture = rt;

        Texture2D screenShot = new(imageWidth, imageHeight, TextureFormat.RGB24, false);
        renderCamera.Render();
        RenderTexture.active = rt;

        screenShot.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        screenShot.Apply();

        byte[] bytes = screenShot.EncodeToJPG(Random.Range((int)(imageQuality * 0.3f), imageQuality));

        Task.Run(() => _mlManager.SavePhotoAndAnnotationAsync(bytes, annots));

        renderCamera.targetTexture = null;
        RenderTexture.active = null;

        Destroy(rt);
        Destroy(screenShot);
        //Debug.Break();
    }

    private List<string> GenerateAnnotations(bool saveBackground = true)
    {
        List<string> annotations = new();
        List<TrainingObject> visibleObjects = GetNearbyObjects();
        print(visibleObjects.Count);

        foreach (TrainingObject obj in visibleObjects)
        {
            YOLOAnnotation? annotation = GetYOLOAnnotation(obj, renderCamera);
            if (annotation is YOLOAnnotation value)
            {
                annotations.Add($"{value.ClassId} {value.CenterX} {value.CenterY} {value.Width} {value.Height}".Replace(',', '.'));

                if (!_names.ContainsKey(obj.GetClassification())) _names.Add(obj.GetClassification(), Array.IndexOf(_trainingData, obj.GetTrainingData()));
            }
        }

        if (annotations.Count == 0 && !saveBackground)
        {
            return null;
        }

        return annotations;
    }

    private List<TrainingObject> GetNearbyObjects()
    {
        List<TrainingObject> visibleObjects = new();
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, maxDistance, _results, _mask);
        for (int i = 0; i < numColliders; i++)
        {
            Collider collider = _results[i];
            if (collider == null) continue;

            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < minDistance) continue;

            if (!collider.TryGetComponent(out TrainingObject trainingObject) || !trainingObject.IsUsingForTraining() ||
                distance >= (_fog ? trainingObject.GetVisibilityRange() * 0.75f : trainingObject.GetVisibilityRange())) continue;

            visibleObjects.Add(trainingObject);
        }

        return visibleObjects;
    }

    private YOLOAnnotation? GetYOLOAnnotation(TrainingObject obj, Camera cam)
    {
        print(2);
        return YOLOAnnotationGenerator.
            TryGetAnnotation(obj, cam, _mask, out YOLOAnnotation annot, Array.IndexOf(_trainingData, obj.GetTrainingData()), true, 3) ? annot : null;
    }

    private void InitializeCameraBase()
    {
        if (!renderCameraInit && renderCamera != null)
        {
            _initialRotation = renderCamera.transform.localRotation;
            _initialFOV = renderCamera.fieldOfView;
            renderCameraInit = true;
        }
    }

    public void RandomizeCamera()
    {
        if (renderCamera == null) return;

        if (!renderCameraInit) InitializeCameraBase();

        float randomYaw = UnityEngine.Random.Range(-maxYawAngle, maxYawAngle);
        float randomPitch = UnityEngine.Random.Range(-maxPitchAngle, maxPitchAngle);

        renderCamera.transform.localRotation = _initialRotation * Quaternion.Euler(randomPitch, randomYaw, 1);

        float newFov = UnityEngine.Random.Range(fovRange.x, fovRange.y);
        renderCamera.fieldOfView = newFov;
    }

    public void ResetCamera()
    {
        if (renderCamera != null && renderCameraInit)
        {
            renderCamera.transform.localRotation = _initialRotation;
            renderCamera.fieldOfView = _initialFOV;
        }
    }
}