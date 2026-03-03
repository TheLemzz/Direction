using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class SimulationStart : MonoBehaviour
{
    [SerializeField] private Image _disableImage;

    private int _sceneIndex;

    private float _clipPlaneVelocity;

    private float _currentAlpha;
    private float _alphaVelocity;

    private bool _started = false;

    private void Start()
    {
        _currentAlpha = _disableImage.color.a;
    }

    public void OnClick()
    {
        _started = true;
        Invoke(nameof(LoadScene), 1.8f);
    }

    private void Update()
    {
        if (!_started) return;

        Camera.main.farClipPlane = Mathf.SmoothDamp(Camera.main.farClipPlane, 1, ref _clipPlaneVelocity, 0.2f);
        _currentAlpha = Mathf.SmoothDamp(_currentAlpha, 2, ref _alphaVelocity, 0.8f);
        _disableImage.color = new Color(_disableImage.color.r, _disableImage.color.g, _disableImage.color.b, _currentAlpha);
    }

    public void SetSceneIndex(int index)
    {
        _sceneIndex = index;
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(_sceneIndex - 1, LoadSceneMode.Single);
        DynamicGI.UpdateEnvironment();
    }
}
