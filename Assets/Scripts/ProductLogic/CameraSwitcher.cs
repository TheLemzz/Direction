using UnityEngine;

public sealed class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _carCamera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _mainCamera.enabled = !_mainCamera.enabled;
            _carCamera.enabled = !_mainCamera.enabled;
        }
    }
}
