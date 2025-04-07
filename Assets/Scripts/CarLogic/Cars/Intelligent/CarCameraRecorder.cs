using System.IO;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class CarCameraRecorder : MonoBehaviour
{
    [SerializeField] private Camera _cameraForRoad;

    private Car _carController;
    private Camera _camera;

    private bool init;
    private string _screenshotPath;

    private void OnEnable()
    {
        EntryPoint.OnApplicationStarted += Init;
    }

    private void OnDisable()
    {
        EntryPoint.OnApplicationStarted -= Init;
    }

    private void Init()
    {
        if (init) return;

        _camera = GetComponent<Camera>();
        _camera.enabled = false;
        init = true;
        _carController = transform.parent.GetComponent<Car>();
        _screenshotPath = $"{Application.dataPath}/Screenshots/{transform.parent.parent.gameObject.name}/";

        Directory.CreateDirectory(_screenshotPath);
    }

    public string MakePhoto(bool forRoad = false, string directory = "")
    {
        Camera camera = forRoad ? _cameraForRoad : _camera;
        if (directory.IsEmpty()) directory = _screenshotPath;

        camera.enabled = true;

        RenderTexture renderTexture = new(Screen.width, Screen.height, 24);
        camera.targetTexture = renderTexture;

        Texture2D screenShot = new(Screen.width, Screen.height, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

        camera.targetTexture = null;
        camera.enabled = false;

        RenderTexture.active = null;
        Destroy(renderTexture);
        byte[] bytes = screenShot.EncodeToJPG(15);
        File.WriteAllBytes(directory + $"{Mathf.RoundToInt(Time.frameCount)}.jpg", bytes);

        _camera.enabled = false;
        return directory;
    }

    private void OnApplicationQuit()
    {
        DeleteScreenshots();
    }

    public void DeleteScreenshots()
    {
        if (init) Directory.Delete(_screenshotPath, true);
    }
}
