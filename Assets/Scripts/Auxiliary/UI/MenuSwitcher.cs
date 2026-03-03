using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public sealed class MenuSwitcher : MonoBehaviour
{
    [SerializeField] private SimulationSwitcherData[] _data;
    [SerializeField] private PostProcessVolume _cameraVolume;
    [SerializeField] private ProductInfoWrapper _wrapper;
    [SerializeField] private SimulationStart _starter;
    [SerializeField] private UISavesLoader _savesLoader;

    private int _index = 0;

    private void Awake()
    {
        ActivateCurrentIndex();
    }

    public void Switch()
    {
        _data[_index].Pos.gameObject.SetActive(false);
        _index = (_index + 1) % _data.Length;

        _data[_index].Pos.gameObject.SetActive(true);
        _cameraVolume.profile = _data[_index].postProcessProfile;
        RenderSettings.skybox = _data[_index].Skybox;

        ActivateCurrentIndex();

        DynamicGI.UpdateEnvironment();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) Switch();

        transform.position = Vector3.Lerp(transform.position, _data[_index].Pos.position, 0.2f);
    }

    private void ActivateCurrentIndex()
    {
        _starter.SetSceneIndex(_data[_index].SceneIndex + 1);
        _wrapper.SetNewProductInfo(_data[_index].ProductInfo);
        _savesLoader.SetFileType(_data[_index].ProductInfo.SaveType);
    }
}
