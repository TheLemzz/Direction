using UnityEngine;

public class AICarLimiter : MonoBehaviour
{
    [SerializeField, Range(1, 10)] private int _coefficentRain;
    [SerializeField, Range(1, 10)] private int _coefficentFog;

    private Cart _cart;

    private void Start()
    {
        _cart = GetComponent<Cart>();
    }

    private void OnEnable()
    {
        WeatherManager.OnWeatherChanged += OnWeatherChanged;
    }

    private void OnDisable()
    {
        WeatherManager.OnWeatherChanged -= OnWeatherChanged;
    }

    private void OnWeatherChanged(bool started, WeatherType weatherType)
    {
        if (started)
        {
            switch (weatherType)
            {
                case WeatherType.Rain:
                    _cart.maxSpeed -= _coefficentRain;
                    break;
                case WeatherType.Fog:
                    _cart.maxSpeed -= _coefficentFog;
                    break;
                default:
                    return;
            }
        }
        else
            switch (weatherType)
            {
                case WeatherType.Rain:
                    _cart.maxSpeed += _coefficentRain;
                    break;
                case WeatherType.Fog:
                    _cart.maxSpeed += _coefficentFog;
                    break;
                default:
                    return;
            }
    }
}
