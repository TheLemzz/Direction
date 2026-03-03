using System;
using System.Collections;
using UnityEngine;

public enum WeatherType
{
    Fog = 0,
    Rain = 1
}

public sealed class WeatherManager : MonoBehaviorSingleton<WeatherManager>
{
    [SerializeField] private GameObject _rainParticleSystem;
    [SerializeField] private GameObject _fogParticleSystem;

    private float _currentFogVelocity;
    private float _currentSkyboxVelocity;

    public static event Action<bool, WeatherType> OnWeatherChanged;

    private void Start()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }

        SetInstance(this);

        StartCoroutine(RainProcess());
        StartCoroutine(FogProcess());
    }

    private void Update()
    {
        RenderSettings.fogDensity = Mathf.SmoothDamp(RenderSettings.fogDensity, IsFogged() ? 0.05f : 0, ref _currentFogVelocity, 2f);
        RenderSettings.skybox.SetFloat("_Exposure", Mathf.SmoothDamp(RenderSettings.skybox.GetFloat("_Exposure"),
            IsFogged() ? 0.45f : 0.82f, ref _currentSkyboxVelocity, 2f));
    }

    private IEnumerator FogProcess()
    {
        WaitForSeconds wait = new(1.5f);

        while (true)
        {
            yield return wait;

            if (!IsFogged() && UnityEngine.Random.Range(0, 100) <= 3)
            {
                SetFog(!IsFogged());
                CancelInvoke(nameof(ChangeFog));
                Invoke(nameof(ChangeFog), UnityEngine.Random.Range(10, 20));
            }
        }

    }

    private IEnumerator RainProcess()
    {
        var wait = new WaitForSeconds(0.5f);

        while (true)
        {
            yield return wait;

            if (!IsRain() && UnityEngine.Random.Range(0, 100) <= 3)
            {
                SetRain(!IsRain());
                CancelInvoke(nameof(ChangeRain));
                Invoke(nameof(ChangeRain), UnityEngine.Random.Range(10, 20));
            }
        }
    }

    private void ChangeRain()
    {
        SetRain(!IsRain());
    }

    private void ChangeFog()
    {
        SetFog(!IsFogged());
    }

    public bool IsRain()
    {
        return _rainParticleSystem.activeSelf;
    }

    public bool IsFogged()
    {
        return _fogParticleSystem.activeSelf;
    }

    public void SetRain(bool value)
    {
        if (IsRain() == value) return;

        _rainParticleSystem.SetActive(value);
        OnWeatherChanged?.Invoke(IsRain(), WeatherType.Rain);
    }

    public void SetFog(bool value)
    {
        if (IsFogged() == value) return;

        _fogParticleSystem.SetActive(value);
        OnWeatherChanged?.Invoke(IsFogged(), WeatherType.Fog);
    }

    private void OnDestroy()
    {
        RenderSettings.skybox.SetFloat("_Exposure", 0.82f);
    }
}
