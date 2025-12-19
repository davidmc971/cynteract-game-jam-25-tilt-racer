using System.Collections.Generic;
using UnityEngine;


public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private List <WheelCollider> _wheelColliders;
    
    public void VisualTransformChange(WheelCollider wheelcollider)
    {
        Transform Visual = wheelcollider.transform.GetChild(0);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
