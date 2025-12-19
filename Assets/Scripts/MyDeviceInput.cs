using Cynteract.InputDevices;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyDeviceInput : MonoBehaviour
{
    MyDevice inputSystemDevice;
    CynteractDevice cynteractDevice;
    private CushionData cushionData;
    void Start()
    {
        CynteractDeviceManager.Instance.ListenOnReady(device =>
        {
            Debug.Log("Device is ready: " + device.Id);
            cushionData = new CushionData(device);
            this.cynteractDevice = device;
            this.cynteractDevice.OnData += OnDeviceDataReceived;
            UnityMainThreadDispatcher.EnqueueAction(() =>
                inputSystemDevice = InputSystem.AddDevice<MyDevice>()
            );
        });

        CynteractDeviceManager.Instance.ListenOnDisconnected(device =>
            {
                Debug.Log("Device disconnected: " + device.Id);
                UnityMainThreadDispatcher.EnqueueAction(() =>
                {
                    if (inputSystemDevice != null)
                    {
                        InputSystem.RemoveDevice(inputSystemDevice);
                        inputSystemDevice = null;
                    }
                });
            });
    }

    void OnDestroy()
    {
        if (inputSystemDevice != null)
        {
            InputSystem.RemoveDevice(inputSystemDevice);
            this.cynteractDevice.OnData -= OnDeviceDataReceived;
        }
    }

    void OnDeviceDataReceived(Connector.Messages.Dataframe data)
    {
        // Debug.Log("Data received from device " + data.imuValues[0].w);
        double xAngle = cushionData.GetXAngle();
        double yAngle = cushionData.GetYAngle();
        double zAngle = cushionData.GetZAngle();

        UnityMainThreadDispatcher.EnqueueAction(() =>
        {
            inputSystemDevice.rotationState = new Vector2((float)yAngle / 90.0f, 1.0f - (float)xAngle / 90.0f);
            inputSystemDevice.buttonState = 0;
        });
        // Debug.Log("Rotation X: " + xAngle + ", Y: " + yAngle + ", Z: " + zAngle);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
