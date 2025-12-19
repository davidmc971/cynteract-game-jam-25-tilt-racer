using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Wheel : MonoBehaviour
{
    [Header("Wheel References")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Movement")]
    public float motorForce = 1500f;
    public float brakeForce = 3000f;
    public float maxSpeed = 25f;
    public float reverseForce = 800f;

    [Header("Steering")]
    public float steerAngle = 30f;
    public float driftSteerMultiplier = 1.7f;

    [Header("Drift")]
    public KeyCode driftKey = KeyCode.LeftShift;
    public float driftGripMultiplier = 0.3f;

    Rigidbody rb;
    float currentSpeed;
    bool isDrifting;

    void Awake() { rb = GetComponent<Rigidbody>(); }

    void FixedUpdate()
    {
        HandleMove();
        HandleSteerAndDrift();
        HandleBrakes();
        UpdateWheelVisuals();
    }

    void HandleMove()
    {
        float inputV = Input.GetAxisRaw("Vertical");
        currentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        float force = 0f;
        if (inputV > 0f && currentSpeed < maxSpeed)
            force = inputV * motorForce;
        else if (inputV < 0f)
            force = inputV * reverseForce;

        // Apply motor to rear wheels
        rearLeftWheel.motorTorque = force;
        rearRightWheel.motorTorque = force;
    }

    void HandleSteerAndDrift()
    {
        float inputH = Input.GetAxisRaw("Horizontal");

        bool driftPressed = Input.GetKey(driftKey);
        isDrifting = driftPressed && Mathf.Abs(inputH) > 0.1f && Mathf.Abs(currentSpeed) > 5f;

        float steerStrength = steerAngle;
        if (isDrifting)
            steerStrength *= driftSteerMultiplier;

        // Speed-based steering
        float speedFactor = Mathf.InverseLerp(0f, maxSpeed, Mathf.Abs(currentSpeed));
        float finalSteer = inputH * steerStrength * speedFactor;

        frontLeftWheel.steerAngle = finalSteer;
        frontRightWheel.steerAngle = finalSteer;
    }

    void HandleBrakes()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            var allWheels = new[] { frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel };
            foreach (var wheel in allWheels)
                wheel.brakeTorque = brakeForce;
        }
        else
        {
            var allWheels = new[] { frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel };
            foreach (var wheel in allWheels)
                wheel.brakeTorque = 0f;
        }
    }

    void UpdateWheelVisuals()
    {
        UpdateSingleWheel(frontLeftWheel, frontLeftWheel.transform.GetChild(0));
        UpdateSingleWheel(frontRightWheel, frontRightWheel.transform.GetChild(0));
        UpdateSingleWheel(rearLeftWheel, rearLeftWheel.transform.GetChild(0));
        UpdateSingleWheel(rearRightWheel, rearRightWheel.transform.GetChild(0));
    }

    void UpdateSingleWheel(WheelCollider collider, Transform visualWheel)
    {
        if (visualWheel == null) return;
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        visualWheel.position = pos;
        visualWheel.rotation = rot * Quaternion.Euler(0, 180, 0);;
    }
}
