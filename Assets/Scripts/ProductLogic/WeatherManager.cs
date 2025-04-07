using System;
using System.Collections;
using UnityEngine;

public enum WeatherType
{
    Fog = 0,
    Rain = 1
}

public sealed class WeatherManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem _rainParticleSystem;
    [SerializeField] private GameObject _fogParticleSystem;

    private float _currentFogVelocity;

    public static event Action<bool, WeatherType> OnWeatherChanged;

    private static WeatherManager _instance;

    public static WeatherManager Instance => _instance;


    private void Update()
    {
        RenderSettings.fogDensity = Mathf.SmoothDamp(RenderSettings.fogDensity, IsFogged() ? 0.05f : 0, ref _currentFogVelocity, 2f);
    }

    public void Init()
    {
        if (_instance == null) _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(RainProcess());
        StartCoroutine(FogProcess());
    }
     
    private IEnumerator FogProcess()
    {
        var wait = new WaitForSeconds(0.75f);

        while (true)
        {
            yield return wait;

            if (!IsFogged() && UnityEngine.Random.Range(0, 100) <= 3)
            {
                SetFog(!IsFogged());
                CancelInvoke(nameof(ChangeFog));
                Invoke(nameof(ChangeFog), 30f);
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
                Invoke(nameof(ChangeRain), 30f);
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
        return _rainParticleSystem.gameObject.activeSelf;
    }

    public bool IsFogged()
    {
        return _fogParticleSystem.gameObject.activeSelf;
    }

    public void SetRain(bool value)
    {
        if (IsRain() == value) return;

        _rainParticleSystem.gameObject.SetActive(value);
        OnWeatherChanged?.Invoke(IsRain(), WeatherType.Rain);
    }

    public void SetFog(bool value)
    {
        if (IsFogged() == value) return;

        _fogParticleSystem.SetActive(value);
        OnWeatherChanged?.Invoke(IsFogged(), WeatherType.Fog);
    }
}
