using UnityEngine;

[RequireComponent(typeof(Car))]
public sealed class CarSpeedLimiter : MonoBehaviour
{
    [SerializeField, Range(1, 20)] private int _coefficentRain;
    [SerializeField, Range(1, 20)] private int _coefficentFog;

    private const string FOG_WARNING = "Туман обнаружен! Будьте бдительны.";
    private const string RAIN_WARNING = "Дождь обнаружен! Будьте бдительны.";

    private CarIntelligentSystem _intelligentSystem;
    private CarController _carController;

    private void Init()
    {
        _carController = GetComponent<CarController>();
        _intelligentSystem = GetComponent<CarIntelligentSystem>();
    }

    private void OnEnable()
    {
        EntryPoint.OnApplicationStarted += Init;
        WeatherManager.OnWeatherChanged += OnWeatherChanged;
    }

    private void OnDisable()
    {
        EntryPoint.OnApplicationStarted -= Init;
        WeatherManager.OnWeatherChanged -= OnWeatherChanged;
    }

    private void OnWeatherChanged(bool started, WeatherType weatherType)
    {
        if (started)
        {
            switch (weatherType)
            {
                case WeatherType.Rain:
                    _carController.AllowedSpeed -= _coefficentRain;
                    _intelligentSystem.SendWarningAllert(RAIN_WARNING);
                    break;
                case WeatherType.Fog:
                    _carController.AllowedSpeed -= _coefficentFog;
                    _intelligentSystem.SendWarningAllert(FOG_WARNING);
                    break;
                default:
                    return;
            }
        }
        else
        switch (weatherType)
        {
            case WeatherType.Rain:
                _carController.AllowedSpeed += _coefficentRain;
                _intelligentSystem.DeleteWarningWithDescription(RAIN_WARNING);
                break;
            case WeatherType.Fog:
                _carController.AllowedSpeed += _coefficentFog;
                _intelligentSystem.DeleteWarningWithDescription(FOG_WARNING);
                break;
            default:
                return;
        }
    }
}
