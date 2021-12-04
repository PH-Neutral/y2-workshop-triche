using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FollowPath;
using static UnityEditor.Experimental.SceneManagement.PrefabStage;

public class TrackPath : MonoBehaviour {
    [System.Serializable]
    public struct Point {
        public float floatIndex;
        [Range(0, 1f)]
        public float widthPercent;
        [Range(0, 0.2f)]
        public float widthRandOffset;

        public int index { get => Mathf.FloorToInt(floatIndex); }
        public float floatPercent { get => floatIndex - Mathf.FloorToInt(floatIndex); }

        public Point(float floatIndex) : this(floatIndex, 0.5f, 0f) { }
        public Point(float floatIndex, float widthPercent, float widthRandOffset) {
            this.floatIndex = floatIndex;
            this.widthPercent = widthPercent;
            this.widthRandOffset = widthRandOffset;
        }
    }

    public int TrackPointsCount { get => track.PointCount; }
    public int RefPointCount { get => refPoints.Length; }
    public Track track;
    public Point[] refPoints;

    public void ChangePoint(int index, Point newPoint) {
        refPoints[index] = newPoint;
    }
    public Vector3 GetTrackPosition(int index, float widthPercent) {
        return track.GetPosition(index, 0f, widthPercent);
    }
    public Vector3 GetPosition(int index) => GetPosition(GetPoint(index));
    public Vector3 GetPosition(Point point) {
        return track.GetPosition(point.floatIndex, point.widthPercent);
    }
    public Point GetPoint(int index) {
        return refPoints[index];
    }

#if UNITY_EDITOR
    /*
    private void OnDrawGizmos() {
        Vector3 origin, target;
        for(int i = 0; i < refPoints.Length; i++) {
            if(refPoints[i].point == null) continue;
            target = refPoints[i].point.position;
            if(i == 0) Gizmos.color = Color.red;
            else Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(target, _gizmoSphereRadius);
            if(i > 0) {
                if(refPoints[i - 1].point == null) continue;
                origin = refPoints[i - 1].point.position;
                if(i == 1) Gizmos.color = Color.red;
                else Gizmos.color = Color.white;
                Gizmos.DrawLine(target, origin);
            }
        }
        if(refPoints.Length > 2 && mode == Mode.Loop) {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(refPoints[0].point.position, refPoints[refPoints.Length - 1].point.position);
        }
    }*/
#endif
}
