using UnityEngine;

public sealed class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    [SerializeField] private Transform _car;

    private void FixedUpdate()
    {
        if (Mathf.Abs(_car.transform.position.x) >= Mathf.Abs(_endPoint.transform.position.x))
        {
            _car.transform.position = _startPoint.transform.position;
        }
    }
}
