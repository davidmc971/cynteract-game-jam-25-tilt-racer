using System.Collections.Generic;
using UnityEngine;


public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private List <WheelCollider> _wheelColliders;
    
    public void VisualTransformChange(WheelCollider wheelcollider)
    {
        Transform Visual = wheelcollider.transform.GetChild(0);
        Vector3 Position;
        Quaternion Rotation;

        wheelcollider.GetWorldPose(out Position, out Rotation);
        Visual.transform.position = Position;
        Visual.transform.rotation = Rotation;
    }
    
    void FixedUpdate()
    {
        foreach (var wheel in _wheelColliders)
        {
            VisualTransformChange(wheel);
        }
    }
}
