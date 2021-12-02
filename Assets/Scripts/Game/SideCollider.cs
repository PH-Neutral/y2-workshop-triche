using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCollider : MonoBehaviour
{
    [SerializeField] bool left;
    Opponent opponent;

    private void Awake()
    {
        opponent = GetComponentInParent<Opponent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        opponent.CallPush(left);
    }
}
