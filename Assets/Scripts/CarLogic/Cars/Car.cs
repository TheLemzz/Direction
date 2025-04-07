using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class Car : MonoBehaviour
{
    [Space(10)]
    [Range(20, 190)]
    public int maxSpeed = 90;
    [Range(10, 120)]
    public int maxReverseSpeed = 45;
    [Range(1, 10)]
    public int accelerationMultiplier = 2;
    [Space(10)]
    [Range(10, 45)]
    public int maxSteeringAngle = 27;
    [Range(0.1f, 1f)]
    public float steeringSpeed = 0.5f;
    [Space(10)]
    [Range(100000, 10000000)]
    public int brakeForce = 100000;
    [Range(1, 10)]
    public int decelerationMultiplier = 2;
    [Range(1, 10)]
    public int handbrakeDriftMultiplier = 5;
    [Space(10)]
    public Vector3 bodyMassCenter;

    [SerializeField] private GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    [Space(10)]
    [SerializeField] private GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    [Space(10)]
    [SerializeField] private GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    [Space(10)]
    [SerializeField] private GameObject rearRightMesh;
    public WheelCollider rearRightCollider;


    [Space(20)]
    [Space(10)]
    [SerializeField] private bool useEffects = false;

    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;

    [Space(10)]
    [SerializeField] private TrailRenderer RLWTireSkid;
    [SerializeField] private TrailRenderer RRWTireSkid;

    [Space(20)]
    [Space(10)]
    [SerializeField] private bool useUI = false;
    [SerializeField] private Text carSpeedText;
    [Space(20)]
    [Space(10)]
    public bool useSounds = false;
    [SerializeField] private AudioSource carEngineSound;
    [SerializeField] private AudioSource tireScreechSound;
    float initialCarEngineSoundPitch;
    [HideInInspector]
    public float carSpeed;
    [HideInInspector]
    public bool isDrifting;
    [HideInInspector]
    public bool isTractionLocked;

    public bool deceleratingCar;
    public float steeringAxis;
    public float localVelocityZ;
    public float localVelocityX;

    private float throttleAxis;
    private float driftingAxis;
    private WheelFrictionCurve FLwheelFriction;
    private float FLWextremumSlip;
    private WheelFrictionCurve FRwheelFriction;
    private float FRWextremumSlip;
    private WheelFrictionCurve RLwheelFriction;
    private float RLWextremumSlip;
    private WheelFrictionCurve RRwheelFriction;
    private float RRWextremumSlip;
    private Rigidbody _carRigidbody;
    private int _cachedSpeed;

    public Rigidbody CarRigidbody => _carRigidbody; 

    private void Start()
    {
        _carRigidbody = gameObject.GetComponent<Rigidbody>();
        _carRigidbody.centerOfMass = bodyMassCenter;
        FLwheelFriction = new WheelFrictionCurve();
        FLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
        FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
        FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
        FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;
        FRwheelFriction = new WheelFrictionCurve();
        FRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
        FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
        FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
        FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;
        RLwheelFriction = new WheelFrictionCurve();
        RLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
        RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
        RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
        RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;
        RRwheelFriction = new WheelFrictionCurve();
        RRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
        RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
        RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
        RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;

        if (carEngineSound != null) initialCarEngineSoundPitch = carEngineSound.pitch;

        if (useUI)
        {
            InvokeRepeating(nameof(CarSpeedUI), 0f, 0.1f);
        }
        else if (!useUI)
        {
            if (carSpeedText != null)
            {
                carSpeedText.text = "0";
            }
        }

        if (useSounds)
        {
            InvokeRepeating(nameof(CarSounds), 0f, 0.1f);
        }
        else if (!useSounds)
        {
            if (carEngineSound != null)
            {
                carEngineSound.Stop();
            }
            if (tireScreechSound != null)
            {
                tireScreechSound.Stop();
            }
        }

        if (!useEffects)
        {
            if (RLWParticleSystem != null)
            {
                RLWParticleSystem.Stop();
            }
            if (RRWParticleSystem != null)
            {
                RRWParticleSystem.Stop();
            }
            if (RLWTireSkid != null)
            {
                RLWTireSkid.emitting = false;
            }
            if (RRWTireSkid != null)
            {
                RRWTireSkid.emitting = false;
            }
        }
    }

    public int GetCarSpeed()
    {
        return Mathf.RoundToInt(Mathf.Abs(carSpeed));
    }

    public void CarSpeedUI()
    {
        if (useUI)
        {
            try
            {
                carSpeedText.text = GetCarSpeed().ToString();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }

    }

    public void CarSounds()
    {

        if (useSounds)
        {
            try
            {
                if (carEngineSound != null)
                {
                    float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(_carRigidbody.velocity.magnitude) / 25f);
                    carEngineSound.pitch = engineSoundPitch;
                }
                if ((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
                {
                    if (!tireScreechSound.isPlaying)
                    {
                        tireScreechSound.Play();
                    }
                }
                else if ((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
                {
                    tireScreechSound.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useSounds)
        {
            if (carEngineSound != null && carEngineSound.isPlaying)
            {
                carEngineSound.Stop();
            }
            if (tireScreechSound != null && tireScreechSound.isPlaying)
            {
                tireScreechSound.Stop();
            }
        }

    }

    public void TurnLeft()
    {
        steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis < -1f)
        {
            steeringAxis = -1f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }
    public void TurnRight()
    {
        steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis > 1f)
        {
            steeringAxis = 1f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    public void ResetSteeringAngle()
    {
        if (steeringAxis < 0f)
        {
            steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        }
        else if (steeringAxis > 0f)
        {
            steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        }
        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            steeringAxis = 0f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    public void AnimateWheelMeshes()
    {
        try
        {
            frontLeftCollider.GetWorldPose(out Vector3 FLWPosition, out Quaternion FLWRotation);
            frontLeftMesh.transform.position = FLWPosition;
            frontLeftMesh.transform.rotation = FLWRotation;

            frontRightCollider.GetWorldPose(out Vector3 FRWPosition, out Quaternion FRWRotation);
            frontRightMesh.transform.position = FRWPosition;
            frontRightMesh.transform.rotation = FRWRotation;

            rearLeftCollider.GetWorldPose(out Vector3 RLWPosition, out Quaternion RLWRotation);
            rearLeftMesh.transform.position = RLWPosition;
            rearLeftMesh.transform.rotation = RLWRotation;

            rearRightCollider.GetWorldPose(out Vector3 RRWPosition, out Quaternion RRWRotation);
            rearRightMesh.transform.position = RRWPosition;
            rearRightMesh.transform.rotation = RRWRotation;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    public void GoForward()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }
        throttleAxis = throttleAxis + (Time.deltaTime * 3f);
        if (throttleAxis > 1f)
        {
            throttleAxis = 1f;
        }
        if (localVelocityZ < -1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.RoundToInt(carSpeed) < maxSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    public void GoReverse()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }
        throttleAxis = throttleAxis - (Time.deltaTime * 3f);
        if (throttleAxis < -1f)
        {
            throttleAxis = -1f;
        }
        if (localVelocityZ > 1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    public void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    public void DecelerateCar()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }
        if (throttleAxis != 0f)
        {
            if (throttleAxis > 0f)
            {
                throttleAxis = throttleAxis - (Time.deltaTime * 10f);
            }
            else if (throttleAxis < 0f)
            {
                throttleAxis = throttleAxis + (Time.deltaTime * 10f);
            }
            if (Mathf.Abs(throttleAxis) < 0.15f)
            {
                throttleAxis = 0f;
            }
        }
        _carRigidbody.velocity = _carRigidbody.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
        if (_carRigidbody.velocity.magnitude < 0.25f)
        {
            _carRigidbody.velocity = Vector3.zero;
            CancelInvoke(nameof(DecelerateCar));
        }
    }

    public void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }

    public void Handbrake()
    {
        CancelInvoke(nameof(RecoverTraction));
        driftingAxis = driftingAxis + (Time.deltaTime);
        float secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

        if (secureStartingPoint < FLWextremumSlip)
        {
            driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
        }
        if (driftingAxis > 1f)
        {
            driftingAxis = 1f;
        }
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }
        if (driftingAxis < 1f)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearRightCollider.sidewaysFriction = RRwheelFriction;
        }
        isTractionLocked = true;
        DriftCarPS();

    }

    public void DriftCarPS()
    {

        if (useEffects)
        {
            try
            {
                if (isDrifting)
                {
                    RLWParticleSystem.Play();
                    RRWParticleSystem.Play();
                }
                else if (!isDrifting)
                {
                    RLWParticleSystem.Stop();
                    RRWParticleSystem.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }

            try
            {
                if ((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
                {
                    RLWTireSkid.emitting = true;
                    RRWTireSkid.emitting = true;
                }
                else
                {
                    RLWTireSkid.emitting = false;
                    RRWTireSkid.emitting = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else if (!useEffects)
        {
            if (RLWParticleSystem != null)
            {
                RLWParticleSystem.Stop();
            }
            if (RRWParticleSystem != null)
            {
                RRWParticleSystem.Stop();
            }
            if (RLWTireSkid != null)
            {
                RLWTireSkid.emitting = false;
            }
            if (RRWTireSkid != null)
            {
                RRWTireSkid.emitting = false;
            }
        }

    }
    public void RecoverTraction()
    {
        isTractionLocked = false;
        driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
        if (driftingAxis < 0f)
        {
            driftingAxis = 0f;
        }

        if (FLwheelFriction.extremumSlip > FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearRightCollider.sidewaysFriction = RRwheelFriction;

            Invoke(nameof(RecoverTraction), Time.deltaTime);

        }
        else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip;
            rearRightCollider.sidewaysFriction = RRwheelFriction;

            driftingAxis = 0f;
        }
    }

}
