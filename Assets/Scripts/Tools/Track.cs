using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Track : MonoBehaviour {

    MeshFilter _mf;
    MeshRenderer _mr;

    private void Awake() {
        _mf = GetComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>();
    }

    public void Setup(Mesh mesh, Material mat) {
        _mf = GetComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>();

        _mf.mesh = mesh;
        _mr.materials = new Material[] { mat };
    }
}
public struct TrackPoint {
    public bool IsStraight { get => Mathf.Abs(angle) == 180; }
    public bool IsRightExt { get => angle >= 0; }
    public Vector2 center;
    public Vector3[] leftPoints;
    public int lastIndexLeft { get => leftPoints.Length - 1; }
    public Vector3 LeftStart { get => leftPoints[0]; }
    public Vector3 LeftEnd { get => leftPoints[leftPoints.Length - 1]; }
    public Vector3[] rightPoints;
    public int lastIndexRight { get => rightPoints.Length - 1; }
    public Vector3 RightStart { get => rightPoints[0]; }
    public Vector3 RightEnd { get => rightPoints[rightPoints.Length - 1]; }

    public float angle;

    public TrackPoint(Vector3 center, Vector3 left, Vector3 right, float angle) : this(center, new Vector3[] { left }, new Vector3[] { right }, angle) { }
    public TrackPoint(Vector3 center, Vector3[] leftPoints, Vector3[] rightPoints, float angle) {
        this.center = center;
        this.leftPoints = leftPoints;
        this.rightPoints = rightPoints;
        this.angle = angle;
    }

    public Vector3 GetPointLooped(bool isRight, int index) {
        return GetPoint(isRight, isRight ? index % rightPoints.Length : index % leftPoints.Length);
    }
    public Vector3 GetPoint(bool isRight, int index) => GetDown(0, isRight, index);
    public Vector3 GetLeft(int index) => GetDown(0, false, index);
    public Vector3 GetLeftDown(float depth, int index) => GetDown(depth, false, index);
    public Vector3 GetLeftStartDown(float depth) => GetDown(depth, false, 0);
    public Vector3 GetLeftEndDown(float depth) => GetDown(depth, false, lastIndexLeft);
    public Vector3 GetRight(int index) => GetDown(0, true, index);
    public Vector3 GetRightDown(float depth, int index) => GetDown(depth, true, index);
    public Vector3 GetRightStartDown(float depth) => GetDown(depth, true, 0);
    public Vector3 GetRightEndDown(float depth) => GetDown(depth, true, lastIndexRight);
    Vector3 GetDown(float depth, bool isRight, int index) {
        if(isRight) {
            return rightPoints[index] + Vector3.down * depth;
        } else {
            return leftPoints[index] + Vector3.down * depth;
        }
    }
}