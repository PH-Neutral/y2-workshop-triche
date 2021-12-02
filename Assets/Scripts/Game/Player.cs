using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider left;
    public WheelCollider right;
    public bool motor;
    public bool steering;
}
public class Player : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque, maxSteeringAngle;
    [SerializeField] Animator leftKickAnimator;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            leftKickAnimator.Play("Kick");
        }
    }
    private void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo a in axleInfos)
        {
            if (a.steering)
            {
                a.left.steerAngle = steering;
                a.right.steerAngle = steering;
            }
            if (a.motor)
            {
                a.left.motorTorque = motor;
                a.right.motorTorque = motor;    
            }
        }
    }
}
