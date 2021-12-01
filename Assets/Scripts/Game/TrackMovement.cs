using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrackMovement : MonoBehaviour {
	public Track spline;
    public float startIndex = 0f;

	int pointIndex;
    float partPercent;
    private void Awake() {
        pointIndex = Mathf.FloorToInt(startIndex);
        partPercent = startIndex - pointIndex;
    }

    float MakeFloatIndex(int index, float percent) {
        return index + percent;
    }
    (int index, float percent) UnmakeFloatIndex(float floatIndex) {
        int pointIndex = Mathf.FloorToInt(startIndex);
        float partPercent = startIndex - pointIndex;
        return (pointIndex, partPercent);
    }

    public void Move(Vector3 localMotion) {
        // convert localMotion in trackProgress

    }
}