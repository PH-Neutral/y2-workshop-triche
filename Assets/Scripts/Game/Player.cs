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
    [SerializeField] float motorToBrakeSensivity = 0.01f;

    Vector3 lastPosition;
    private void Awake()
    {
        lastPosition = transform.position;
    }
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            leftKickAnimator.Play("Kick");
        }
    }
    */
    private void Update()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo a in axleInfos)
        {
            Debug.Log("brake : " + (a.left.brakeTorque == 0 ? 0 : 1));
            Debug.Log("motor : " + (a.left.motorTorque == 0 ? 0 : 1));
            if (a.steering)
            {
                a.left.steerAngle = steering;
                a.right.steerAngle = steering;
            }
            if (a.motor)
            {
                if (transform.InverseTransformDirection(transform.position - lastPosition).z > motorToBrakeSensivity)
                {
                    //Debug.Log("A");
                    if (motor >= 0)
                    {
                        //Debug.Log("A1");
                        a.left.motorTorque = motor;
                        a.right.motorTorque = motor;
                    }
                    else
                    {
                        //Debug.Log("A2");
                        //a.left.motorTorque = a.right.motorTorque = 0;
                        a.left.brakeTorque = Mathf.Abs(motor);
                        a.right.brakeTorque = Mathf.Abs(motor);
                    }
                }
                else if (transform.InverseTransformDirection(transform.position - lastPosition).z < -motorToBrakeSensivity)
                {
                    //Debug.Log("B");
                    if (motor <= 0)
                    {
                        //Debug.Log("B1");
                        a.left.motorTorque = motor;
                        a.right.motorTorque = motor;
                    }
                    else
                    {
                        //Debug.Log("B2");
                        //a.left.motorTorque = a.right.motorTorque = 0;
                        a.left.brakeTorque = Mathf.Abs(motor);
                        a.right.brakeTorque = Mathf.Abs(motor);
                    }
                }
                else
                {
                    a.left.brakeTorque = a.right.brakeTorque = 0;
                    a.left.motorTorque = motor;
                    a.right.motorTorque = motor;
                }
            }
        }


        lastPosition = transform.position;
    }
}
