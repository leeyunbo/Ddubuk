using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SplineType
{
    CatmullromSpline,
    LinearSpline,
}

public abstract class Spline
{

    /// <summary>
    /// The points interpolated of the spline.
    /// </summary>
    public Vector3[] points { get; protected set; }

    /// <summary>
    /// The CatmullRom curve-segments of the spline.
    /// </summary>
    protected Curve[] segments;

    /// <summary>
    /// The number of segments that make up the spline.
    /// </summary>
    protected int segmentCount = 0;

    /// <summary>
    /// The full (estimated) length of the spline.
    /// </summary>
    public float Length { get; protected set; }

    protected float[] lengths;

    /// <summary>
    /// Calculate the catmull-rom segments. Also estimates the curve's length.
    /// </summary>
    /// <param name="N">The number sample points used to estimate each segment's length.</param>
    public abstract void CalculateSegments(int N);

    /// <summary>
    /// Returns the point of the spline at a given arc-length.
    /// </summary>
    /// <param name="s">The arc-length.</param>
    /// <returns></returns>
    public Vector3 GetPointAtArcLength(float s)
    {
        s = Mathf.Clamp(s, 0, Length);

        for (var i = 0; i < segmentCount; i++)
        {
            if (s <= lengths[i])
            {
                var offset = i == 0 ? 0 : lengths[i - 1];
                return segments[i].GetPointAtLength(s - offset);
            }
        }
        return segments[segmentCount - 1].GetPoint(1);
    }

    /// <summary>
    /// Returns a CurvePointData whith the point and tangent of the spline
    /// at a given arc-length.
    /// </summary>
    /// <param name="s">The arc-length.</param>
    /// <returns></returns>
    public CurvePointData GetPointAndTangentAtArcLength(float s)
    {
        s = Mathf.Clamp(s, 0, Length);

        for (var i = 0; i < segmentCount; i++)
        {
            if (s <= lengths[i])
            {
                var offset = i == 0 ? 0 : lengths[i - 1];
                return segments[i].GetPointAndTangentAtLength(s - offset);
            }
        }
        return segments[segmentCount - 1].GetPointAndTangentAtLength(1);
    }

    /// <summary>
    /// Draws the curve using a given LineRenderer, with points being processed by a given
    /// function beforehand.
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="func"></param>
    public void DrawCurveWithLineRenderer(LineRenderer renderer, System.Func<Vector3, Vector3> func, int N = 100)
    {
        var points = new List<Vector3>();

        float s = 0.0f;
        while (s <= Length)
        {
            var pointData = GetPointAndTangentAtArcLength(s);
            points.Add(func(pointData.point));
            s += Length / (N + 1.0f);
        }

        var arr = points.ToArray();
        renderer.positionCount = arr.Length;
        renderer.SetPositions(arr);
    }

    /// <summary>
    /// Calculates a sample of (N+2) equidistant points along the spline.
    /// </summary>
    /// <param name="N">The number of points in the sample will be (N+2).</param>
    /// <param name="func">A function that can be used to transform the sampled poins.</param>
    /// <returns></returns>
    public Vector3[] SamplePoints(int N, System.Func<Vector3, Vector3> func)
    {
        var sample = new Vector3[N + 2];
        var delta = Length / (N + 1.0f);

        var s = 0.0f;
        for (var i = 0; i < (N + 2); i++)
        {
            sample[i] = func(GetPointAtArcLength(s));
            s += delta;
        }

        return sample;
    }

    /// <summary>
    /// Calculates a sample of (N+2) equidistant points along the spline.
    /// </summary>
    /// <param name="N">The number of points in the sample will be (N+2).</param>
    /// <returns></returns>
    public Vector3[] SamplePoints(int N)
    {
        return SamplePoints(N, (p) => p);
    }

    /// <summary>
    /// Draw the curve and sample point using Gizmos.
    /// </summary>
    public void DrawGizmos()
    {
        DrawPointsGizmos();
        DrawCurveLengthGizmos();
    }

    private void DrawPointsGizmos()
    {
        foreach (var p in points)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(p, 0.1f);
        }
    }

    private void DrawCurveLengthGizmos()
    {
        var p = GetPointAtArcLength(0f);
        float s = 0.0f;
        while (s <= Length)
        {
            Gizmos.color = Color.green;
            var pointData = GetPointAndTangentAtArcLength(s);
            Vector3 n = pointData.point;
            Gizmos.DrawLine(p, n);
            p = n;
            s += 0.1f;
            Gizmos.color = Color.magenta;
            var tan = pointData.tangent;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(n, n + tan);
        }
    }
}
