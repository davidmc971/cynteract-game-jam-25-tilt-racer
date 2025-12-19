using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ArcadeCartController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 25f;
    public float reverseAcceleration = 15f;
    public float maxSpeed = 25f;
    public float maxReverseSpeed = 10f;
    public float steering = 80f;
    public float grip = 5f;

    [Header("Drift")]
    public KeyCode driftKey = KeyCode.LeftShift;
    public float driftSteerMultiplier = 1.7f;
    public float driftGripMultiplier = 0.3f;

    Rigidbody rb;
    float currentSpeed;
    bool isDrifting;
    InputAction moveAction;

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.Enable();

        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HandleMove();
        HandleSteerAndDrift();
        ApplyGrip();
    }

    void HandleMove()
    {
        float inputForwardBackward = moveAction.ReadValue<Vector2>().y;
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        currentSpeed = Vector3.Dot(rb.linearVelocity, flatForward);

        float targetAccel = 0f;

        if (inputForwardBackward > 0f)
        {
            if (currentSpeed < maxSpeed)
                targetAccel = inputForwardBackward * acceleration;
        }
        else if (inputForwardBackward < 0f)
        {
            if (inputForwardBackward > -maxReverseSpeed)
                targetAccel = inputForwardBackward * reverseAcceleration;
        }

        rb.AddForce(flatForward * targetAccel, ForceMode.Acceleration);
    }

    void HandleSteerAndDrift()
    {
        float inputLeftRight = moveAction.ReadValue<Vector2>().x;

        bool driftPressed = Input.GetKey(driftKey);
        isDrifting = driftPressed && Mathf.Abs(inputLeftRight) > 0.1f && Mathf.Abs(currentSpeed) > 5f;

        float steerStrength = steering;
        if (isDrifting)
            steerStrength *= driftSteerMultiplier;

        // Steering based on speed so you can't spin in place
        float speedFactor = Mathf.InverseLerp(0f, maxSpeed, Mathf.Abs(currentSpeed));
        float turnAmount = inputLeftRight * steerStrength * speedFactor * Time.fixedDeltaTime;

        Quaternion turnOffset = Quaternion.Euler(0f, turnAmount * Mathf.Sign(currentSpeed == 0 ? 1 : currentSpeed), 0f);
        rb.MoveRotation(rb.rotation * turnOffset);
    }

    void ApplyGrip()
    {
        Vector3 vel = rb.linearVelocity;
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        // Decompose velocity into forward and sideways components
        float forwardSpeed = Vector3.Dot(vel, flatForward);
        Vector3 forwardVel = flatForward * forwardSpeed;
        Vector3 sidewaysVel = vel - forwardVel;

        float gripAmount = grip;
        if (isDrifting)
            gripAmount *= driftGripMultiplier;

        // Dampen sideways velocity for grip / drift
        sidewaysVel = Vector3.Lerp(sidewaysVel, Vector3.zero, gripAmount * Time.fixedDeltaTime);

        rb.linearVelocity = forwardVel + sidewaysVel;
    }
}
