using UnityEngine;

[RequireComponent(typeof(Car))]
public sealed class PlayerCarInputHandler : CarController
{
    private Car _carController;

    private void Start()
    {
        _carController = GetComponent<Car>();
    }

    private void Update()
    {
        base.Handle();

        if (!IgonreRule && _carController.GetCarSpeed() >= AllowedSpeed && !_carController.deceleratingCar)
        {
            _carController.InvokeRepeating(nameof(Car.DecelerateCar), 0f, 0.1f);
            _carController.deceleratingCar = true;
        }

        else if (Input.GetKey(KeyCode.W))
        {
            _carController.CancelInvoke(nameof(Car.DecelerateCar));
            _carController.deceleratingCar = false;
            _carController.GoForward();
        }

        ProcessInput();

        _carController.AnimateWheelMeshes();
    }

    private void ProcessInput()
    {
        if (Input.GetKey(KeyCode.S))
        {
            _carController.CancelInvoke(nameof(Car.DecelerateCar));
            _carController.deceleratingCar = false;
            _carController.GoReverse();
        }

        if (Input.GetKey(KeyCode.A))
        {
            _carController.TurnLeft();
        }

        if (Input.GetKey(KeyCode.D))
        {
            _carController.TurnRight();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _carController.CancelInvoke(nameof(Car.DecelerateCar));
            _carController.deceleratingCar = false;
            _carController.Handbrake();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _carController.RecoverTraction();
        }

        if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            _carController.ThrottleOff();
        }

        if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !_carController.deceleratingCar)
        {
            _carController.InvokeRepeating(nameof(Car.DecelerateCar), 0f, 0.1f);
            _carController.deceleratingCar = true;
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && _carController.steeringAxis != 0f)
        {
            _carController.ResetSteeringAngle();
        }
    }

    public void ChangeVaule()
    {
        _ignoreRule = !_ignoreRule;
    }
}
