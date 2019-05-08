using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A (open-ended) catmull-rom spline, which interpolates a set points by joining 
/// catmull-rom curves together.
/// </summary>
public class CatmullRomSpline : Spline
{

    /// <summary>
    /// The start-point control/handle.
    /// </summary>
    private Vector3 startHandle;

    /// <summary>
    /// The end-point control/handle.
    /// </summary>
    private Vector3 endHandle;

    /// <summary>
    /// The alpha/tension parameter of the spline.
    /// </summary>
    public float Alpha
    {
        get
        {
            return alpha;
        }
        set
        {
            alpha = value;
            CalculateSegments(lastSampleSize);
        }
    }

    float alpha;
    int lastSampleSize;

    /// <summary>
    /// Creates a new Catmull-rom spline.
    /// </summary>
    /// <param name="points">The interpolated points.</param>
    /// <param name="N">The number of samples used in each segment of the spline.</param>
    public CatmullRomSpline(Vector3[] points, int N, float alpha)
    {
        this.points = (Vector3[]) points.Clone();
        this.alpha = alpha;

        CalculateSegments(N);
    }

    /// <summary>
    /// Calculate the catmull-rom segments. Also estimates the curve's length.
    /// </summary>
    /// <param name="N">The number sample points used to estimate each segment's length.</param>
    public override void CalculateSegments(int N = 100)
    {
        lastSampleSize = N;

        segmentCount = (points.Length - 1);
        lengths = new float[segmentCount];

        segments = new CatmullRomCurve[segmentCount];

        startHandle = 2 * points[0] - points[1];
        endHandle = 2 * points[segmentCount] - points[segmentCount - 1];

        Length = 0;
        for (var i = 0; i < segmentCount; i++)
        {
            segments[i] = new CatmullRomCurve(
                i == 0 ? startHandle : points[i - 1],
                points[i],
                points[i + 1],
                (i + 1) == segmentCount ? endHandle : points[i + 2],
                Alpha
            );

            Length += segments[i].EstimateLength(N);
            lengths[i] = Length;
        }
    }

}