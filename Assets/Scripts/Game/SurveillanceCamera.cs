using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveillanceCamera : MonoBehaviour
{
    [SerializeField] Animator iconCamAnimator;
    [SerializeField] float sightRadius, cautionRadius;

    bool IsPlayerInSight
    {
        get
        {
            return _isPlayerInSight;
        }
        set
        {
            _isPlayerInSight = value;
            iconCamAnimator.SetBool("InSight", value);
        }
    }
    bool _isPlayerInSight;
    bool IsPlayerInCaution
    {
        get
        {
            return _isPlayerInCaution;
        }
        set
        {
            _isPlayerInCaution = value;
            iconCamAnimator.SetBool("InCaution", value);
        }
    }
    bool _isPlayerInCaution;
    RaycastHit hit;


    private void Update()
    {
        Debug.Log(IsPlayerInCaution);
        if (Physics.SphereCast(transform.position, sightRadius, Vector3.down, out hit, 200, 1<< LayerMask.NameToLayer("Player")))
        {
            IsPlayerInSight = true;
        }
        else if (Physics.SphereCast(transform.position, cautionRadius, Vector3.down, out hit, 200,1<< LayerMask.NameToLayer("Player")))
        {
            IsPlayerInSight = false;
            IsPlayerInCaution = true;
        }
        else
        {
            IsPlayerInCaution = IsPlayerInSight = false;
        }
    }
    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, sightRadius);
        Gizmos.DrawSphere(transform.position + (Vector3.down * 5), sightRadius);
    }
    */
}