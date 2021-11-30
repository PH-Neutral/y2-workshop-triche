using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrackGenerator))]
public class TrackGeneratorEditor : Editor {
    TrackGenerator gen;
    bool canGenerate;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        gen = (TrackGenerator)target;
        canGenerate = true;

        EditorGUILayout.Space();
        if(!gen.IsSplineSet) {
            canGenerate = false;
            EditorGUILayout.HelpBox("The spline needs to be set!", MessageType.Error);
        }
        if(!gen.IsWidthValid || !gen.IsDepthValid) {
            canGenerate = false;
            EditorGUILayout.HelpBox("Width and Depth must be greater than 0.", MessageType.Error);
        }
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.enabled = canGenerate;
        bool doGenerate = GUILayout.Button("   Generate   ");
        GUI.enabled = true;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if(doGenerate) {
            gen.Generate();
        }
    }
}
