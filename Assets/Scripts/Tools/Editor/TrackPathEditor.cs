using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrackPath))]
public class TrackPathEditor : Editor {
    /*/
    const float directionScale = 0.5f, handleSize = 0.04f, pickSize = 0.06f;

    TrackPath path;
    Transform handleTransform;
    Quaternion handleRotation;
    int selectedIndex = -1;

    public override void OnInspectorGUI() {
        path = target as TrackPath;
        if(selectedIndex >= 0 && selectedIndex < path.RefPointCount) {
            DrawSelectedPointInspector();
        }
    }
    
    private void OnSceneGUI() {
        path = target as TrackPath;
        handleTransform = path.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
        if(path.TrackPointsCount == 0) return;

        Vector3 p0 = ShowPoint(0, path.GetPoint(0).widthPercent), p1;
        for(int i = 0; i < path.TrackPointsCount; i++) {
            p1 = ShowPoint(i < path.TrackPointsCount - 1 ? i + 1 : 0);
            Handles.color = Color.white;
            Handles.DrawLine(p0, p1, 2f);
            p0 = p1;
        }
    }

    public float ShowPath(TrackPath.Point startPoint) {
        int startIndex = startPoint.index;
        int endIndex = startPoint.index - 1;
        int cutIndex = startIndex > endIndex ? path.TrackPointsCount : endIndex + 1;
        int loopedIndex;
        bool firstPart = startIndex > endIndex; // to reset the for as to keep the order of indexes
        float distance = 0f;
        Vector3 p0, p1;
        for(int i = startIndex; i < cutIndex; i++) {
            loopedIndex = path.track.LoopIndex(i);
            if(i == startIndex) {
                p0 = path.track.GetPosition(loopedIndex, startPoint.floatPercent, startPoint.widthPercent);
                p1 = path.track.GetPosition(loopedIndex + 1, 0f, widthPercent);
                //distance += Vector3.Distance(p0, p1);
            } else if(i == endIndex) {
                //p0 = path.track.GetPosition(loopedIndex, 0f, widthPercent);
                p1 = path.track.GetPosition(loopedIndex + 1, floatIndex1.percent, widthPercent);
                //distance += Vector3.Distance(p0, p1);
            } else {
                p0 = path.track.GetPosition(loopedIndex, 0f, widthPercent);
                p1 = path.track.GetPosition(loopedIndex + 1, 0f, widthPercent);
                //distance += Vector3.Distance(p0, p1);
            }
            if(i == 0) p0 = ShowPoint(i);
            p1 = ShowPoint(i < path.TrackPointsCount - 1 ? i + 1 : 0);
            Handles.color = Color.white;
            Handles.DrawLine(p0, p1, 2f);
            p0 = p1;

            // end loop
            if(firstPart && i == path.TrackPointsCount - 1) {
                i = 0; cutIndex = endIndex;
                firstPart = false;
            }
        }
        return distance;
    }

    Vector3 ShowPoint(int index, float widthPercent) => ShowPoint(index, widthPercent, Color.white);
    Vector3 ShowPoint(int index, float widthPercent, Color color) {
        Vector3 point = handleTransform.TransformPoint(path.GetTrackPosition(index, widthPercent));
        float size = HandleUtility.GetHandleSize(point);
        if(index == 0) {
            size *= 2f;
        }
        Handles.color = color;
        if(Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap)) {
        }
        return point;
    }
    private void DrawSelectedPointInspector() {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        TrackPath.Point newPoint = path.GetPoint(selectedIndex);
        newPoint.floatIndex = EditorGUILayout.FloatField("Pos Length", newPoint.floatIndex);
        newPoint.widthPercent = EditorGUILayout.FloatField("Pos Width", newPoint.widthPercent);
        newPoint.widthRandOffset = EditorGUILayout.FloatField("Rand Width", newPoint.widthRandOffset);
        if(EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(path, "Move Point");
            EditorUtility.SetDirty(path);
            path.ChangePoint(selectedIndex, newPoint);
        }
    }//*/
}