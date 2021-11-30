using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Track : MonoBehaviour {
    public bool isLooping;
    [SerializeField] Vector3[] leftPoints, rightPoints;
    public int PointCount { get => leftPoints.Length; }

    MeshFilter _mf;
    MeshRenderer _mr;

    private void Awake() {
        _mf = GetComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>();
    }

    public void SetupRendering(Mesh mesh, Material mat) {
        _mf = GetComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>();

        _mf.mesh = mesh;
        _mr.materials = new Material[] { mat };
    }
    public void Setup(TrackPoint[] points, bool isLooping) {
        this.isLooping = isLooping;
        List<Vector3> leftList = new List<Vector3>();
        List<Vector3> rightList = new List<Vector3>();
        TrackPoint tp;
        int posArrayLength;
        for(int i = 0; i < points.Length; i++) {
            tp = points[i];
            posArrayLength = Mathf.Max(tp.leftPoints.Length, tp.rightPoints.Length);
            for(int j = 0; j < posArrayLength; j++) {
                if(isLooping && i == points.Length - 1 && j == posArrayLength - 1) {
                    break;
                }
                leftList.Add(tp.GetPointLooped(false, j));
                rightList.Add(tp.GetPointLooped(true, j));
            }
        }
        leftPoints = leftList.ToArray();
        rightPoints = rightList.ToArray();
    }

    public float GetDistance((int index, float percent) floatIndex0, (int index, float percent) floatIndex1, 
        float widthPercent, bool isMainDir = true) {
        int startIndex = isMainDir ? floatIndex0.index : floatIndex1.index;
        int endIndex = isMainDir ? floatIndex1.index : floatIndex0.index;
        int cutIndex = startIndex > endIndex ? PointCount : endIndex + 1;
        int loopedIndex;
        bool firstPart = startIndex > endIndex; // to reset the for as to keep the order of indexes
        float distance = 0f;
        Vector3 p0, p1;
        for(int i = startIndex; i < cutIndex; i += isMainDir ? 1 : -1) {
            loopedIndex = LoopIndex(i);
            if(i == startIndex) {
                p0 = GetPoint(loopedIndex, floatIndex0.percent, widthPercent);
                p1 = GetPoint(loopedIndex + 1, 0f, widthPercent);
                distance += Vector3.Distance(p0, p1);
            } else if(i == endIndex) {
                p0 = GetPoint(loopedIndex, 0f, widthPercent);
                p1 = GetPoint(loopedIndex + 1, floatIndex1.percent, widthPercent);
                distance += Vector3.Distance(p0, p1);
            } else {
                p0 = GetPoint(loopedIndex, 0f, widthPercent);
                p1 = GetPoint(loopedIndex + 1, 0f, widthPercent);
                distance += Vector3.Distance(p0, p1);
            }
            // end loop
            if(firstPart && i == PointCount - 1) {
                i = 0; cutIndex = endIndex;
                firstPart = false;
            }
        }
        return distance;
    }
    public float GetDistance1(int index, float widthPercent0, float widthPercent1) {
        return Vector3.Distance(
            GetPoint(LoopIndex(index), 0f, widthPercent0), 
            GetPoint(LoopIndex(index + 1), 0f, widthPercent1));
    }
    public Vector3 GetPoint(int index, float percent, float widthPercent) => GetPoint((index, percent), widthPercent);
    public Vector3 GetPoint((int index, float percent) floatIndex, float widthPercent) {
        int i = floatIndex.index % PointCount;
        if(i < 0) i += PointCount;
        return Vector3.Lerp(
            Vector3.Lerp(leftPoints[i], rightPoints[i], floatIndex.percent),
            Vector3.Lerp(leftPoints[i + 1], rightPoints[i + 1], floatIndex.percent), 
            widthPercent);
    }
    public int LoopIndex(int index) {
        int i = index % PointCount; 
        if(index < 0) index += PointCount;
        return i;
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