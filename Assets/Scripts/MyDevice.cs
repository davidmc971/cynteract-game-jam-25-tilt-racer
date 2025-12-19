using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;


public struct MyDeviceState : IInputStateTypeInfo
{
    // FourCC type codes are used to identify the memory layouts of state blocks.
    public FourCC format => new FourCC('M', 'D', 'E', 'V');

    [InputControl(name = "shakeButton", layout = "Button", bit = 0)]
    public int buttons;
    [InputControl(name = "rotation", layout = "Vector2")]
    public Vector2 rotation;
}

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(displayName = "My Device", stateType = typeof(MyDeviceState))]
public class MyDevice : InputDevice, IInputUpdateCallbackReceiver
{
    public ButtonControl shakeButton { get; private set; }
    public Vector2Control rotation { get; private set; }

    public int buttonState;
    public Vector2 rotationState;

    static MyDevice()
    {
        InputSystem.RegisterLayout<MyDevice>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterMyDevice()
    {
        InputSystem.RegisterLayout<MyDevice>();
    }

    protected override void FinishSetup()
    {
        shakeButton = GetChildControl<ButtonControl>("shakeButton");
        rotation = GetChildControl<Vector2Control>("rotation");
        base.FinishSetup();
    }

    public void OnUpdate()
    {
        var state = new MyDeviceState
        {
            buttons = buttonState,
            rotation = rotationState
        };
        InputSystem.QueueStateEvent(this, state);
    }
}
