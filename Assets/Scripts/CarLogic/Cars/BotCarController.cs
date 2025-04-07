using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Car), typeof(NavMeshAgent))]
[Obsolete]
public sealed class BotCarController : CarController
{
    [SerializeField] private Transform _target;

    private Car _carController;
    private NavMeshAgent _ai;

    private void Start()
    {
        _carController = GetComponent<Car>();
        _ai = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        base.Handle();

        //if (_carController.GetCarSpeed() < AllowedSpeed)
        //{
        //    _carController.CancelInvoke(nameof(Car.DecelerateCar));
        //    _carController.deceleratingCar = false;
        //    _carController.GoForward();
        //}

        //else if(!_carController.deceleratingCar)
        //{
        //    _carController.InvokeRepeating(nameof(Car.DecelerateCar), 0f, 0.1f);
        //    _carController.deceleratingCar = true;
        //}
        if (_target != null) _ai.SetDestination(_target.position);
        _carController.AnimateWheelMeshes();
    }
}
