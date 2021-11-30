using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoForward : MonoBehaviour
{
    [SerializeField] float speed = 1;
    void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }
}
