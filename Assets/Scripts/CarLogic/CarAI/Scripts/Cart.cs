using UnityEngine;

public class Cart : MonoBehaviour
{
    public WheelCollider[] wheels;
    public Transform[] wheels_mesh;
    public float wheel_torque = 200;
    public float brake_torque = 500;
    public float max_steerangle = 30;

    private Vector3 wheel_position;
    private Quaternion wheel_rotation;

    public float current_speed;

    [HideInInspector]
    public Rigidbody _rigidbody;

    private Vector3 savedPauseVelocity;
    private Vector3 savedPauseAngularVelocity;

    public bool isAICart;
    public bool isBrake;
    public float maxSpeed;

    private float _currentVelocity;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isBrake) _rigidbody.velocity /= 2;
    }

    public void AccelerateCart(float v, float h, float b)
    {
        current_speed = isBrake ? Mathf.SmoothDamp(current_speed, 0, ref _currentVelocity, 0.02f) : Mathf.RoundToInt(_rigidbody.velocity.magnitude * 3.6f);
        if (v > 0)
        {
            if (current_speed < maxSpeed && !isBrake)
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = Mathf.Clamp(v, -1f, 1f) * wheel_torque;
                }
            }
            else
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = 0;
                }
            }
        }
        else if (v < 0)
        {
            if (current_speed > -5)
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = Mathf.Clamp(v, -1f, 1f) * wheel_torque;
                }
            }
            else
            {
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = 0;
                }
            }
        }

        if (v == 0 && !isAICart)
        {
            b = 0.3f;
        }

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].brakeTorque = Mathf.Clamp(b, 0f, 1f) * brake_torque;
        }

        wheels[0].steerAngle = Mathf.Clamp(h, -1f, 1f) * max_steerangle;
        wheels[1].steerAngle = Mathf.Clamp(h, -1f, 1f) * max_steerangle;
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].GetWorldPose(out wheel_position, out wheel_rotation);
            wheels_mesh[i].position = wheel_position;
            wheels_mesh[i].rotation = wheel_rotation;
        }
    }

    public void CheckForSkid()
    {
        for (int i = 2; i < 4; i++)
        {
            WheelHit wheelHit;
            wheels[i].GetGroundHit(out wheelHit);
        }
    }

    public void OnPause(bool pause)
    {
        if (pause)
        {
            savedPauseVelocity = _rigidbody.velocity;
            savedPauseAngularVelocity = _rigidbody.angularVelocity;
            if (_rigidbody.isKinematic == false)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }
        else
        {
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = savedPauseVelocity;
            _rigidbody.angularVelocity = savedPauseAngularVelocity;
        }
    }
}
