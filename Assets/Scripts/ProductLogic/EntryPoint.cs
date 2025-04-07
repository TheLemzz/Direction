using System;
using UnityEngine;

[RequireComponent(typeof(WeatherManager), typeof(Teleporter), typeof(CameraSwitcher))]
public sealed class EntryPoint : MonoBehaviour
{
    private WeatherManager _weatherManager;
    private PyModule _module;

    public static event Action OnApplicationStarted;

    private void Start()
    {
        Application.targetFrameRate = 90;
        _weatherManager = GetComponent<WeatherManager>();
        _module = GetComponent<PyModule>();

        _module.Init();
        _weatherManager.Init();

        OnApplicationStarted?.Invoke();
    }
}
