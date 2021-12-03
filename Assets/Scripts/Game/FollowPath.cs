using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour {

    public bool showGizmos = true;
    public Transform[] waypoints = new Transform[0];
    [SerializeField] float _gizmoSphereRadius = 0.5f;

    public int GetNextIndex(int currentIndex) {
        int nextIndex = (currentIndex + 1) % waypoints.Length;
        if(nextIndex < 0) nextIndex += waypoints.Length;
        else if(nextIndex >= waypoints.Length) nextIndex -= waypoints.Length;
        return nextIndex;
    }
    public Transform GetWaypoint(int index) {
        return waypoints[index];
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if(!showGizmos) return;
        Vector3 origin, target;
        for(int i = 0; i < waypoints.Length; i++) {
            if(waypoints[i] == null) continue;
            target = waypoints[i].position;
            if(i == 0) Gizmos.color = Color.red;
            else Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(target, _gizmoSphereRadius);
            if(i > 0) {
                if(waypoints[i - 1] == null) continue;
                origin = waypoints[i - 1].position;
                if(i == 1) Gizmos.color = Color.red;
                else Gizmos.color = Color.white;
                Gizmos.DrawLine(target, origin);
            }
        }
        if(waypoints.Length > 2) {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(waypoints[0].position, waypoints[waypoints.Length - 1].position);
        }
    }
#endif
}