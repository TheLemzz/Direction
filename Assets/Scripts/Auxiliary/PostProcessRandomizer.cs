using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public class PostProcessRandomizer : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Как часто менять параметры (в секундах)")]
    [SerializeField] private float changeInterval = 8.0f;

    [Header("Debug")]
    [SerializeField] private bool runOnStart = true;

    private PostProcessVolume _volume;
    private float _timer = 0;

    private Grain _grain;
    private AmbientOcclusion _ambientOcclusion;
    private ChromaticAberration _chromaticAberration;
    private LensDistortion _lensDistortion;
    private ColorGrading _colorGrading;

    private Vignette _vignette;
    private Bloom _bloom;
    private MotionBlur _motionBlur;

    private void Awake()
    {
        _volume = GetComponent<PostProcessVolume>();

        InitEffects();

        if (runOnStart) ChangeEffects();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= changeInterval)
        {
            ChangeEffects();
            _timer = 0;
        }
    }

    private void InitEffects()
    {
        if (_volume.profile == null)
        {
            Debug.LogError("В PostProcessVolume не назначен профиль!");
            enabled = false;
            return;
        }

        _volume.profile.TryGetSettings(out _grain);
        _volume.profile.TryGetSettings(out _ambientOcclusion);
        _volume.profile.TryGetSettings(out _chromaticAberration);
        _volume.profile.TryGetSettings(out _lensDistortion);
        _volume.profile.TryGetSettings(out _colorGrading);

        _volume.profile.TryGetSettings(out _vignette);
        _volume.profile.TryGetSettings(out _bloom);
        _volume.profile.TryGetSettings(out _motionBlur);
    }

    public void ChangeEffects()
    {
        if (_grain != null)
        {
            _grain.active = true;
            _grain.intensity.value = Random.Range(0.15f, 0.98f);
            _grain.size.value = Random.Range(0.6f, 1.5f);
            _grain.colored.value = Random.value >= 0.5f;

            _grain.intensity.overrideState = true;
            _grain.size.overrideState = true;
            _grain.colored.overrideState = true;
        }

        if (_ambientOcclusion != null)
        {
            _ambientOcclusion.active = true;
            _ambientOcclusion.intensity.value = Random.Range(0.5f, 1.5f);
            _ambientOcclusion.intensity.overrideState = true;
        }

        if (_chromaticAberration != null)
        {
            _chromaticAberration.active = true;
            _chromaticAberration.intensity.value = Random.Range(0.01f, 0.4f);
            _chromaticAberration.intensity.overrideState = true;
        }

        if (_lensDistortion != null)
        {
            _lensDistortion.active = true;
            _lensDistortion.intensity.value = Random.Range(-39f, 39f);

            float scaleFix = 1f;
            if (Mathf.Abs(_lensDistortion.intensity.value) > 20) scaleFix = Random.Range(0.8f, 1.1f);
            _lensDistortion.scale.value = scaleFix;

            _lensDistortion.intensity.overrideState = true;
        }

        if (_colorGrading != null)
        {
            _colorGrading.active = true;
            _colorGrading.temperature.value = Random.Range(-12f, 12f);

            _colorGrading.postExposure.value = Random.Range(-0.5f, 0.85f);

            _colorGrading.contrast.value = Random.Range(-10f, 25f);

            _colorGrading.temperature.overrideState = true;
            _colorGrading.postExposure.overrideState = true;
            _colorGrading.contrast.overrideState = true;
        }

        if (_vignette != null)
        {
            _vignette.active = true;
            _vignette.intensity.value = Random.Range(0.01f, 0.25f);
            _vignette.smoothness.value = Random.Range(0f, 1f);
            _vignette.intensity.overrideState = true;
        }

        if (_bloom != null)
        {
            _bloom.active = true;
            _bloom.intensity.value = Random.Range(0.8f, 8.5f);
            _bloom.threshold.value = Random.Range(0.8f, 1.2f);
            _bloom.diffusion.value = Random.Range(3f, 7.5f);
            _bloom.intensity.overrideState = true;
        }


    }
}