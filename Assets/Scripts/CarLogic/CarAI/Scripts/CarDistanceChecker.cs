using UnityEngine;

public class CarDistanceChecker : MonoBehaviour
{
    [SerializeField] private bool _sendAllert;
    [SerializeField] private bool _forceStop;

    [SerializeField] private Car _car;
    [SerializeField] private Cart _aiCar;
    [SerializeField] private CarIntelligentSystem _warningSystem;

    private const string WARNING = "Опасное сближение!";

    private void FixedUpdate()
    {
        Ray ray = new(transform.position, transform.forward);
        Debug.DrawRay(transform.position, transform.forward);

        if (Physics.Raycast(ray, 10, LayerMask.GetMask("Car")))
        {

            if (_sendAllert) _warningSystem.SendWarningAllert(WARNING);
            if (_forceStop)
            {
                if (_car != null) _car.Handbrake();
                else _aiCar.isBrake = true;
            }
        }
        else
        {
            if (_aiCar != null) _aiCar.isBrake = false;
            if (_warningSystem != null) _warningSystem.DeleteWarningWithDescription(WARNING);
        }
    }
}
