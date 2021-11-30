using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour
{
    [SerializeField] bool x, y, z;
    Vector3 eulerAngles;

    private void Awake()
    {
        eulerAngles = transform.eulerAngles;
    }

    private void Update()
    {
        if (x) transform.rotation = Quaternion.Euler(eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        if (y) transform.rotation = Quaternion.Euler(transform.eulerAngles.x, eulerAngles.y, transform.eulerAngles.z);
        if (z) transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, eulerAngles.z);
    }
}
