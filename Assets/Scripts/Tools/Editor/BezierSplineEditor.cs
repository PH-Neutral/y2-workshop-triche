using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineEditor : Editor {
    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };
    const int stepsPerCurve = 10; 
    const float directionScale = 0.5f, handleSize = 0.04f, pickSize = 0.06f;

    BezierSpline spline;
    Transform handleTransform;
    Quaternion handleRotation;
    int selectedIndex = -1;

    public override void OnInspectorGUI() {
        spline = target as BezierSpline;
        if(selectedIndex >= 0 && selectedIndex < spline.ControlPointCount) {
            DrawSelectedPointInspector();
        }
        spline.segmentMode = EditorGUILayout.Toggle("Segment Mode ?", spline.segmentMode);
        spline.isLooping = EditorGUILayout.Toggle("Is Looping ?", spline.isLooping);
        EditorGUILayout.Space();

        if(GUILayout.Button("Add Curve")) {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }
    }
    private void OnSceneGUI() {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
        Vector3 pEnd = ShowPoint(spline.ControlPointCount - 1);
        Vector3 p0 = ShowPoint(0); 
        if(spline.isLooping && spline.segmentMode) {
            Handles.color = Color.white;
            Handles.DrawLine(pEnd, p0, 2f);
        }
        for(int i = 1; i < spline.ControlPointCount; i += 3) {
            Vector3 p1, p2, p3 = ShowPoint(i + 2);


            if(spline.segmentMode) {
                Handles.color = Color.white;
                Handles.DrawLine(p0, p3, 2f);
            } else {
                p1 = ShowPoint(i);
                p2 = ShowPoint(i + 1);
                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);
                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            }
            p0 = p3;
        }
        if(!spline.segmentMode) ShowDirections();
    }
    private void ShowDirections() {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        int steps = stepsPerCurve * spline.CurveCount;
        for(int i = 1; i <= steps; i++) {
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
    }
    Vector3 ShowPoint(int index) {
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
        float size = HandleUtility.GetHandleSize(point);
        if(index == 0) {
            size *= 2f;
        }
        Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
        if(Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap)) {
            selectedIndex = index;
            Repaint();
        }
        if(selectedIndex == index) {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
            }
        }
        return point;
    }
    private void DrawSelectedPointInspector() {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
        if(EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectedIndex, point);
        }
        EditorGUI.BeginChangeCheck();
        Bezier.ControlPointMode mode = (Bezier.ControlPointMode)
            EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
        if(EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Change Point Mode");
            spline.SetControlPointMode(selectedIndex, mode);
            EditorUtility.SetDirty(spline);
        }
    }
}