using UnityEngine;
using UnityEngine.AI;

public sealed class CarDistanceChecker : MonoBehaviour
{
    [SerializeField, Tooltip("���������� �� ��������������")] private bool sendAlert;
    [SerializeField, Tooltip("������������� ������������� ������")] private bool forceStop;

    [SerializeField, Tooltip("������ ������")] private Car playerCar;
    [SerializeField, Tooltip("������ � ��")] private Cart aiCar;
    [SerializeField, Tooltip("������� ��������������")] private CarIntelligentSystem warningSystem;

    private const string WARNING_MESSAGE = "������� ���������!";
    private const string CAR_LAYER = "Car";
    private const string PEOPLE_LAYER = "People";

    private int carLayerMask;
    private int peopleLayerMask;
    private const float RAYCAST_DISTANCE = 10f;

    private void Start()
    {
        carLayerMask = LayerMask.GetMask(CAR_LAYER);
        peopleLayerMask = LayerMask.GetMask(PEOPLE_LAYER);
    }

    private void FixedUpdate()
    {
        Ray ray = new(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * RAYCAST_DISTANCE);

        if (Physics.Raycast(ray, out RaycastHit hit, RAYCAST_DISTANCE, carLayerMask))
        {
            ProcessStop();
            return;
        }

        if (Physics.Raycast(ray, out hit, RAYCAST_DISTANCE, peopleLayerMask) && hit.rigidbody != null && hit.rigidbody.TryGetComponent(out NavMeshAgent agent) && agent.velocity.magnitude > 0)
        {
            ProcessStop();
            return;
        }

        ResetCarState();
    }

    private void ProcessStop()
    {
        if (sendAlert && warningSystem != null)
        {
            warningSystem.SendWarningAlert(WARNING_MESSAGE);
        }

        if (forceStop)
        {
            if (playerCar != null)
            {
                playerCar.Handbrake();
            }
            else if (aiCar != null)
            {
                aiCar.isBrake = true;
            }
        }
    }

    private void ResetCarState()
    {
        if (aiCar != null) aiCar.isBrake = false;

        if (warningSystem != null) warningSystem.DeleteWarningWithDescription(WARNING_MESSAGE);
    }
}

