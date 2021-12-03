using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Vector3 positionRelativeToPlayer = new Vector3(0,3,-4);
    void Update()
    {
        transform.position = player.transform.TransformPoint(positionRelativeToPlayer);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, player.transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
