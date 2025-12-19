using UnityEngine;
using UnityEngine.InputSystem;

public class InputPrinter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputAction moveAction;
    InputAction attackAction;
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");

        moveAction.Enable();
        attackAction.Enable();
    }

    void OnDestroy()
    {
        moveAction.Disable();
        attackAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // Read Move input (Vector2)
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // Read Attack input (Button)
        bool attackInput = attackAction.IsPressed();
        // Print the inputs (only when there's input to avoid spam)
        Debug.Log($"Move: {moveInput}, Attack: {attackInput}");
    }
}
