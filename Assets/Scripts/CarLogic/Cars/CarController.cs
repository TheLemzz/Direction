using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CarIntelligentSystem))]
public class CarController : MonoBehaviour
{
    [SerializeField] protected bool _ignoreRule = false;
    [SerializeField] private Text _allowedSpeedText;
    [SerializeField, Range(10, 100)] private float _allowedSpeedInit;

    private CarIntelligentSystem _intelligentSystem;
    private Car _car;

    private float _internalAllowedSpeed;

    public bool IgonreRule => _ignoreRule;

    public float AllowedSpeed
    {
        get
        {
            return _internalAllowedSpeed;
        }
        set
        {
            _internalAllowedSpeed = Mathf.Clamp(value, 1, _allowedSpeedInit);
        }
    }

    private void OnEnable()
    {
        EntryPoint.OnApplicationStarted += OnProjectStart;
    }

    private void OnDisable()
    {
        EntryPoint.OnApplicationStarted -= OnProjectStart;
    }

    private void OnProjectStart()
    {
        _internalAllowedSpeed = _allowedSpeedInit;
        _intelligentSystem = GetComponent<CarIntelligentSystem>();
        _car = GetComponent<Car>();

        InitComponents();
    }

    private void InitComponents()
    {
        _intelligentSystem.Init();
    }

    private void FixedUpdate()
    {
        if (_allowedSpeedText != null) _allowedSpeedText.text = AllowedSpeed.ToString();
    }

    protected void Handle()
    {
        _car.carSpeed = 2 * Mathf.PI * _car.frontLeftCollider.radius * _car.frontLeftCollider.rpm * 60 / 1000;
        _car.localVelocityX = transform.InverseTransformDirection(_car.CarRigidbody.velocity).x;
        _car.localVelocityZ = transform.InverseTransformDirection(_car.CarRigidbody.velocity).z;
    }
}
