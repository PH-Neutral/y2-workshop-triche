using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using Color = UnityEngine.Color;

public class TrackGenerator : MonoBehaviour {
    const string trackName = "Track";
    public bool IsSplineSet { get => spline != null; }
    public bool IsWidthValid { get => width > 0; }
    public bool IsDepthValid { get => depth > 0; }
    float HalfWidth { get => width * 0.5f; }

    [SerializeField] BezierSpline spline;
    [Range(0, 5f)]
    public float width = 1;
    [Range(0, 1f)]
    public float outerLength = 0;
    [Range(0, 1f)]
    public float depth = 0.5f;
    [Range(0, 5)]
    public int cornerPrecision = 1;
    public Material baseMat;
    public bool copyMaterial = false;
    public bool createObject = false;

    // +++ mesh stuff +++ //
    List<Vector3> vertices, normals;
    List<Vector2> uvs;
    List<int> triangles;

    [SerializeField] bool showGizmos = false;
    List<Vector3> drawLines = new List<Vector3>(), drawNormals = new List<Vector3>(); 

    public void Generate() {
        Debug.Log("Generating Track...");
        // --- Initialize variables --- //
        Mesh mesh = new Mesh {
            name = $"{trackName} Mesh"
        };
        vertices = new List<Vector3>();
        triangles = new List<int>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();

        // debug
        drawLines = new List<Vector3>();
        drawNormals = new List<Vector3>();

        // --- Calculate Mesh Data --- //
        List<TrackPoint> trackPoints = new List<TrackPoint>();
        TrackPoint lastPoint = CalculateTrackPoint(spline.isLooping ? spline.CurveCount : 0), point;
        trackPoints.Add(lastPoint);
        for(int i = spline.isLooping ? 0 : 1; i <= spline.CurveCount; i++) {
            point = CalculateTrackPoint(i);
            trackPoints.Add(point);
            MakeMeshPart(lastPoint, point);
            lastPoint = point;
        }
        // --- Initialize Mesh --- //
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh = RecalculateTangents(mesh);

        // --- instantiate object in scene with mesh --- //
        if(!createObject) return;
        Track track;
        if((track = GetComponentInChildren<Track>()) == null) {
            GameObject obj = new GameObject(trackName);
            obj.transform.SetParent(transform, false);
            track = obj.AddComponent<Track>();
        }
        track.SetupRendering( mesh, copyMaterial ? Instantiate(baseMat) : baseMat);
        track.Setup(trackPoints.ToArray(), spline.isLooping);
    }
    void MakeMeshPart(TrackPoint point0, TrackPoint point1) {
        Vector3 p0LeftDown = point0.GetLeftEndDown(depth), p0RightDown = point0.GetRightEndDown(depth);
        Vector3 p1LeftDown = point1.GetLeftStartDown(depth), p1RightDown = point1.GetRightStartDown(depth);
        Vector3 halfDown = Vector3.down * depth * 0.5f;

        Vector3 outward0Left = point0.LeftEnd + halfDown + (point0.LeftEnd - point0.RightEnd).normalized * outerLength;
        Vector3 outward0Right = point0.RightEnd + halfDown - (point0.LeftEnd - point0.RightEnd).normalized * outerLength;
        Vector3 outward1Left = point1.LeftStart + halfDown + (point1.LeftStart - point1.RightStart).normalized * outerLength;
        Vector3 outward1Right = point1.RightStart + halfDown - (point1.LeftStart - point1.RightStart).normalized * outerLength;

        // --- Make "straight" track part --- //
        if(point0.IsRightExt) {
            outward0Left = point0.LeftEnd + halfDown
                + ((point0.LeftEnd - point0.RightEnd).normalized + (point0.LeftStart - point0.RightStart).normalized).normalized
                * outerLength;
        } else {
            outward0Right = point0.RightEnd + halfDown
                - ((point0.LeftEnd - point0.RightEnd).normalized + (point0.LeftStart - point0.RightStart).normalized).normalized
                * outerLength;
        }
        if(point1.IsRightExt) {
            outward1Left = point1.LeftEnd + halfDown
                + ((point1.LeftEnd - point1.RightEnd).normalized + (point1.LeftStart - point1.RightStart).normalized).normalized
                * outerLength;
        } else {
            outward1Right = point1.RightEnd + halfDown
                - ((point1.LeftEnd - point1.RightEnd).normalized + (point1.LeftStart - point1.RightStart).normalized).normalized
                * outerLength;
        }
        // side up
        AddQuad(point0.LeftEnd, point0.RightEnd, point1.LeftStart, point1.RightStart);
        // side right
        AddQuad(point0.RightEnd, outward0Right, point1.RightStart, outward1Right);
        AddQuad(outward0Right, p0RightDown, outward1Right, p1RightDown);
        // side left
        AddQuad(outward0Left, point0.LeftEnd, outward1Left, point1.LeftStart);
        AddQuad(p0LeftDown, outward0Left, p1LeftDown, outward1Left);
        // side down
        AddQuad(p0RightDown, p0LeftDown, p1RightDown, p1LeftDown);

        // --- Make "corner" track part --- //
        if(!point1.IsStraight) {
            if(cornerPrecision == 0) {
                return;
            }
            Vector3 outwardExt0, outwardExt1;
            for(int i = 1; i < cornerPrecision + 2; i++) {
                if(point1.IsRightExt) {
                    outwardExt0 = point1.GetRight(i - 1) + halfDown + (point1.GetRight(i - 1) - point1.GetLeft(0)).normalized * outerLength;
                    outwardExt1 = point1.GetRight(i) + halfDown + (point1.GetRight(i) - point1.GetLeft(0)).normalized * outerLength;
                    // side up
                    AddTriangle(
                        point1.LeftStart, point1.rightPoints[i - 1], point1.rightPoints[i],
                        Vector2.right, Vector2.zero, Vector2.one);
                    // side out
                    AddQuad(point1.rightPoints[i - 1], outwardExt0, point1.rightPoints[i], outwardExt1);
                    AddQuad(outwardExt0, point1.GetRightDown(depth, i - 1), outwardExt1, point1.GetRightDown(depth, i));
                    // side down
                    AddTriangle(
                        point1.GetRightDown(depth, i - 1), point1.GetLeftStartDown(depth), point1.GetRightDown(depth, i),
                        Vector2.right, Vector2.zero, Vector2.one);
                } else {
                    outwardExt0 = point1.GetLeft(i - 1) + halfDown + (point1.GetLeft(i - 1) - point1.GetRight(0)).normalized * outerLength;
                    outwardExt1 = point1.GetLeft(i) + halfDown + (point1.GetLeft(i) - point1.GetRight(0)).normalized * outerLength;
                    // side up
                    AddTriangle(
                        point1.leftPoints[i - 1], point1.RightStart, point1.leftPoints[i],
                        Vector2.right, Vector2.zero, Vector2.one);
                    // side out
                    AddQuad(outwardExt0, point1.leftPoints[i - 1], outwardExt1, point1.leftPoints[i]);
                    AddQuad(point1.GetLeftDown(depth, i - 1), outwardExt0, point1.GetLeftDown(depth, i), outwardExt1);
                    // side down
                    AddTriangle(
                        point1.GetRightStartDown(depth), point1.GetLeftDown(depth, i - 1), point1.GetLeftDown(depth, i),
                        Vector2.right, Vector2.zero, Vector2.one);
                }
            }
        }
    }
    TrackPoint CalculateTrackPoint(int pointIndex) {
        Vector3 lastPoint, point, nextPoint, dirLast, dirNext;
        if(!spline.isLooping && pointIndex <= 0) {
            // if 1st point of spline (if not looping)
            point = spline.GetPoint(pointIndex);
            nextPoint = spline.GetPoint(pointIndex + 1);
            lastPoint = point - (nextPoint - point);
        } else if(!spline.isLooping && pointIndex >= spline.CurveCount) {
            // if last point of spline (if not looping)
            lastPoint = spline.GetPoint(pointIndex - 1);
            point = spline.GetPoint(pointIndex);
            nextPoint = point - (lastPoint - point);
        } else {
            // if it's not a spline end
            lastPoint = spline.GetPoint(pointIndex - 1);
            point = spline.GetPoint(pointIndex);
            nextPoint = spline.GetPoint(pointIndex + 1);
        }
        dirLast = lastPoint - point; dirNext = nextPoint - point;
        float angle = Vector3.SignedAngle(dirLast, dirNext, Vector3.up);
        if(Mathf.Abs(angle) == 180) {
            // if the track and its neighbors are aligned => straight track
            return MakeStraightTrackPart(point, dirLast, angle);
        } else {
            // if the track and its neighbors aren't aligned => corner
            return MakeCornerTrackPart(point, dirLast, dirNext, angle);
        }
        //return GenerateTrackPoint(point, dirLast, dirNext, angle);
    }
    TrackPoint MakeStraightTrackPart(Vector3 point, Vector3 dirLast, float angle) {
        Vector3 startSideLeft = Quaternion.AngleAxis(90f, Vector3.up) * dirLast.normalized * HalfWidth;
        // debug
        drawLines.Add(point + startSideLeft);
        drawLines.Add(point - startSideLeft);
        // ---
        return new TrackPoint(
            center: point,
            left: point + startSideLeft,
            right: point - startSideLeft,
            angle: angle
            );
    }
    TrackPoint MakeCornerTrackPart(Vector3 point, Vector3 dirLast, Vector3 dirNext, float angle) {
        Vector3 vSide = (dirLast.normalized + dirNext.normalized).normalized
            * (HalfWidth / Mathf.Sin(Mathf.Abs(angle) * 0.5f * Mathf.Deg2Rad));
        float extAngle = 180f - Mathf.Abs(angle);
        float partAngle = extAngle / (cornerPrecision + 1f);
        float angleOffset = cornerPrecision > 0 ? -extAngle * 0.5f : 0;

        bool rightIsExt = angle >= 0;
        Vector3[] leftPoints = new Vector3[rightIsExt || cornerPrecision == 0 ? 1 : 2 + cornerPrecision];
        Vector3[] rightPoints = new Vector3[rightIsExt && cornerPrecision != 0 ? 2 + cornerPrecision : 1];
        leftPoints[0] = point + vSide;
        rightPoints[0] = point + vSide;
        Vector3 segment;
        for(int i = 0; i < Mathf.Max(rightPoints.Length, leftPoints.Length); i++) {
            segment = Quaternion.AngleAxis(i * partAngle + angleOffset, rightIsExt ? Vector3.down : Vector3.up)
                * -vSide * 2;
            if(rightIsExt) {
                rightPoints[i] = leftPoints[0] + segment;
                // debug
                drawLines.Add(leftPoints[0]);
                drawLines.Add(rightPoints[i]);
                // ---
            } else {
                leftPoints[i] = rightPoints[0] + segment;
                // debug
                drawLines.Add(leftPoints[i]);
                drawLines.Add(rightPoints[0]);
                // ---
            }
        }
        return new TrackPoint(
            center: point,
            leftPoints: leftPoints,
            rightPoints: rightPoints,
            angle: angle
            );
    }
    TrackPoint GenerateTrackPoint(Vector3 point, Vector3 dirLast, Vector3 dirNext, float trackAngle) {
        if(Mathf.Abs(trackAngle) == 180) {
            // if the track point is aligned with last and next point
            Vector3 startSideLeft = Quaternion.AngleAxis(90f, Vector3.up) * dirLast.normalized * HalfWidth;
            // debug
            drawLines.Add(point + startSideLeft);
            drawLines.Add(point - startSideLeft);
            // ---
            return new TrackPoint(
                center: point,
                left: point + startSideLeft,
                right: point - startSideLeft,
                angle: trackAngle
                );
        } else {
            Vector3 vSide = (dirLast.normalized + dirNext.normalized).normalized
                * (HalfWidth / Mathf.Sin(Mathf.Abs(trackAngle) * 0.5f * Mathf.Deg2Rad));
            float extAngle = 180f - Mathf.Abs(trackAngle);
            float partAngle = extAngle / (cornerPrecision + 1f);
            float angleOffset = cornerPrecision > 0 ? -extAngle * 0.5f : 0;

            bool rightIsExt = trackAngle >= 0;
            Vector3[] leftPoints = new Vector3[rightIsExt || cornerPrecision == 0 ? 1 : 2 + cornerPrecision];
            Vector3[] rightPoints = new Vector3[rightIsExt && cornerPrecision != 0 ? 2 + cornerPrecision : 1];
            leftPoints[0] = point + vSide;
            rightPoints[0] = point + vSide;
            Vector3 segment;
            for(int i = 0; i < Mathf.Max(rightPoints.Length, leftPoints.Length); i++) {
                segment = Quaternion.AngleAxis(i * partAngle + angleOffset, rightIsExt ? Vector3.down : Vector3.up)
                    * -vSide * 2;
                if(rightIsExt) {
                    rightPoints[i] = leftPoints[0] + segment;
                    // debug
                    drawLines.Add(leftPoints[0]);
                    drawLines.Add(rightPoints[i]);
                    // ---
                } else {
                    leftPoints[i] = rightPoints[0] + segment;
                    // debug
                    drawLines.Add(leftPoints[i]);
                    drawLines.Add(rightPoints[0]);
                    // ---
                }
            }
            return new TrackPoint(
                center: point,
                leftPoints: leftPoints,
                rightPoints: rightPoints,
                angle: trackAngle
                );
        }
    }
    void AddQuad(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight) {
        Vector3 normal = FindNormal(bottomRight - bottomLeft, topLeft - bottomLeft);
        int i0 = AddVertice(bottomLeft, normal, Vector2.zero);
        int i1 = AddVertice(bottomRight, normal, Vector2.right);
        int i2 = AddVertice(topLeft, normal, Vector2.up);
        int i3 = AddVertice(topRight, normal, Vector2.one);
        triangles.Add(i0);
        triangles.Add(i2);
        triangles.Add(i1);
        triangles.Add(i1);
        triangles.Add(i2);
        triangles.Add(i3);
    }
    void AddTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Vector2 uv0, Vector2 uv1, Vector2 uv2) {
        Vector3 normal = FindNormal(p1 - p0, p2 - p0);
        int i0 = AddVertice(p0, normal, uv0);
        int i1 = AddVertice(p1, normal, uv1);
        int i2 = AddVertice(p2, normal, uv2);
        triangles.Add(i0);
        triangles.Add(i2);
        triangles.Add(i1);
    }
    int AddVertice(Vector3 pos, Vector3 normal, Vector2 uv) {
        vertices.Add(pos);
        normals.Add(normal);
        uvs.Add(uv);

        return vertices.Count - 1;
    }
    Vector3 FindNormal(Vector3 dir1, Vector3 dir2) {
        return Vector3.Cross(dir1, dir2).normalized;
    }

    public static Mesh RecalculateTangents(Mesh mesh) {
        int triangleCount = mesh.triangles.Length / 3;
        int vertexCount = mesh.vertices.Length;

        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];
        Vector4[] tangents = new Vector4[vertexCount];
        int a = 0;
        while(a < triangleCount) {
            long i1 = mesh.triangles[a++];
            long i2 = mesh.triangles[a++];
            long i3 = mesh.triangles[a++];

            Vector3 v1 = mesh.vertices[i1];
            Vector3 v2 = mesh.vertices[i2];
            Vector3 v3 = mesh.vertices[i3];

            Vector2 w1 = mesh.uv[i1];
            Vector2 w2 = mesh.uv[i2];
            Vector2 w3 = mesh.uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }
        for(a = 0; a < vertexCount; a++) {
            Vector3 n = mesh.normals[a];
            Vector3 t = tan1[a];
            tangents[a] = (t - n * Vector3.Dot(n, t)).normalized;
            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }
        mesh.tangents = tangents;
        return mesh;
    }

    private void OnDrawGizmos() {
        if(!showGizmos) return;

        Color defColor = Gizmos.color;
        Gizmos.color = Color.cyan;
        int cornerCount = (cornerPrecision + 2) * 2;
        int offset = 2;
        for(int i = 1; i < drawLines.Count; i += 2) {
            Gizmos.DrawLine(drawLines[i], drawLines[i - 1]);
            if((i - offset) % 2 == 1) {
                if(i - offset > 1 && ((i - offset) % cornerCount) - 2 > 0) {
                    Gizmos.DrawLine(drawLines[i], drawLines[i - 2]);
                }
                if(i - offset > 0 && (i - offset) % cornerCount == 1) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(drawLines[i], drawLines[i - 3]);
                    Gizmos.DrawLine(drawLines[i - 1], drawLines[i - 2]);
                    Gizmos.color = Color.cyan;
                }
            }
        }
        Gizmos.color = Color.green;
        for(int i = 0; i < drawNormals.Count; i += 2) {
            Gizmos.DrawLine(drawNormals[i], drawNormals[i + 1]);
        }

        Gizmos.color = defColor;
    }
}