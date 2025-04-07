using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CarController), typeof(CarSpeedLimiter), typeof(Car))]
public sealed class CarIntelligentSystem : MonoBehaviour
{
    [SerializeField] private WarningAlertHolder _warningAllertHolder;
    [SerializeField] private CarCameraRecorder _cameraRecorder;
    [SerializeField] private TextMeshProUGUI _roadQualityText;
    [SerializeField] private bool _detect = true;
    [SerializeField] private bool _sendData = true;

    private CarController _carController;
    private Car _car;
    private PyModule _module;

    private const string SPEED_WARNING = "Скорость превышена!";

    private bool init = false;
    private float _roadQuality;

    public float RoadQuality => _roadQuality;

    private const string OUTPUT_FILE_PATH = @"E:\UnityProjects\siriusinternal\AI\output.txt";


    public void UpdateRoadQuality()
    {
        try
        {
            if (File.Exists(OUTPUT_FILE_PATH))
            {
                string text = File.ReadAllText(OUTPUT_FILE_PATH);
                if (float.TryParse(text.Replace('.', ','), out float roadQualityValue))
                {
                    _roadQuality = roadQualityValue;
                }
                else
                {
                    Debug.LogError($"Failed to parse float from file: {OUTPUT_FILE_PATH}");
                }
            }
            else
            {
                Debug.LogError($"File not found: {OUTPUT_FILE_PATH}");
            }
        }

        catch (System.Exception e)
        {
            Debug.LogError($"Error reading road quality: {e.Message}");
        }
    }

    private IEnumerator UpdateRoadQualityCoroutine()
    {
        WaitForSeconds wait = new(10);

        while (true)
        {
            yield return wait;

            UpdateRoadQuality();
            _roadQualityText.text = _roadQuality.ToString();
            if (_roadQualityText.text == "1") _roadQualityText.text = "1.00";
        }
    }

    public void Init()
    {
        if (init) return;
        init = true;

        _car = GetComponent<Car>();
        _carController = GetComponent<CarController>();
        _module = PyModule.Instance;

        if (!_detect && !_sendData) _cameraRecorder.GetComponent<Camera>().enabled = false;
        else
        {
            StartCoroutine(SendRoadData());
        }

        UpdateRoadQuality();
        StartCoroutine(UpdateRoadQualityCoroutine());
    }

    public void SendWarningAlert(string description)
    {
        _warningAllertHolder.AddWarningAlert(description);
    }

    public void DeleteWarningWithDescription(string data = SPEED_WARNING)
    {
        _warningAllertHolder.TryRemoveWarningAlert(data);
    }

    private void FixedUpdate()
    {

        if (!init) return;

        if (_carController.IgonreRule) CheckSpeed();

        if (_car.GetCarSpeed() < _carController.AllowedSpeed)
        {
            DeleteWarningWithDescription();
        }
    }

    private void CheckSpeed()
    {
        if (_car.GetCarSpeed() > _carController.AllowedSpeed && !_warningAllertHolder.IsHasWarningWithDescription(SPEED_WARNING))
        {
            SendWarningAlert(SPEED_WARNING);
        }
    }

    private IEnumerator SendRoadData()
    {
        WaitForSeconds wait = new(2.2f);

        while (true)
        {
            yield return wait;
            TrySendRoadData();
        }
    }

    private void TrySendRoadData()
    {
        if (_sendData && _car.GetCarSpeed() >= 3) _cameraRecorder.MakePhoto(true, _module.GetRoadDataPath());
    }
}
