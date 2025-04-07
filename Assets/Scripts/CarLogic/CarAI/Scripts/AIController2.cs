using UnityEngine;
using UnityEngine.AI;

public class AIController2 : MonoBehaviour
{
    public Circuit circuit;
    private Cart cart;
    public float steeringSensitivity = 0.01f;
    public float breakingSensitivity = 1.0f;
    public float accelerationSensitivity = 0.3f;

    public GameObject trackerPrefab;
    private NavMeshAgent agent;

    private int currentTrackerWP;
    private float lookAhead = 10;

    private const float cornerAngleThreshold = 20f;
    private const float speedFactorThreshold = 0.1f;
    private const float maxSpeed = 30f;

    private float sqrLookAhead;

    private Vector3 _localTarget;

    private void Start()
    {
        cart = GetComponent<Cart>();
        GameObject tracker = Instantiate(trackerPrefab, transform.position, transform.rotation);
        agent = tracker.GetComponent<NavMeshAgent>();
        sqrLookAhead = lookAhead * lookAhead;
    }

    private void ProgressTracker()
    {
        if ((agent.transform.position - transform.position).sqrMagnitude > sqrLookAhead)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(circuit.waypoints[currentTrackerWP].position);

        if (Vector3.Distance(agent.transform.position, circuit.waypoints[currentTrackerWP].position) < 4)
        {
            currentTrackerWP = (currentTrackerWP + 1) % circuit.waypoints.Count;
        }
    }

    private void Update()
    {
        agent.isStopped = cart.isBrake;

        ProgressTracker();

        _localTarget = transform.InverseTransformPoint(agent.transform.position);
        float targetAngle = Mathf.Atan2(_localTarget.x, _localTarget.z) * Mathf.Rad2Deg;

        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        float cornerFactor = corner / 90f;

        float accel = 1f;
        if (corner > cornerAngleThreshold && cart.current_speed / maxSpeed > speedFactorThreshold)
            accel = Mathf.Lerp(0, 1 * accelerationSensitivity, 1 - cornerFactor);


        cart.AccelerateCart(accel, Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(cart.current_speed), 0);
    }
}
