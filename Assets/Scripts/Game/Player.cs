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
    public GameObject grapple;
    public Transform spawn;
    public bool isInSight;
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque, maxSteeringAngle, turboTorque;
    [SerializeField] Animator leftKickAnimator, rightKickAnimator;
    [SerializeField] float motorToBrakeSensivity = 0.01f;
    [SerializeField] GameObject smoke, fire, smoke2, fire2;

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

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (isInSight) UIManager.Instance.Loose();
            leftKickAnimator.Play("KickLeft");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInSight) UIManager.Instance.Loose();
            rightKickAnimator.Play("KickRight");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isInSight) UIManager.Instance.Loose();
            smoke.SetActive(false);
            fire.SetActive(true);
            smoke2.SetActive(false);
            fire2.SetActive(true);
            maxMotorTorque += turboTorque;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isInSight) UIManager.Instance.Loose();
            smoke.SetActive(true);
            fire.SetActive(false);
            smoke2.SetActive(true);
            fire2.SetActive(false);
            maxMotorTorque -= turboTorque;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Instantiate(grapple, spawn.position, transform.rotation);
        }

        foreach (AxleInfo a in axleInfos)
        {
            //Debug.Log("brake : " + (a.left.brakeTorque == 0 ? 0 : 1));
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
                        a.left.motorTorque = 0;
                        a.right.motorTorque = 0;
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
                        a.left.motorTorque = 0;
                        a.right.motorTorque = 0;
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

    IEnumerator Death()
    {
        yield return new WaitForSeconds(0.2f);
        UIManager.Instance.Loose();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Collision")
        {
            StartCoroutine(Death());
        }
    }
}
