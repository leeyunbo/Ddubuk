using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearSpline : Spline
{
    public LinearSpline(Vector3[] points)
    {
        this.points = (Vector3[]) points.Clone();

        CalculateSegments(100);
    }

    public override void CalculateSegments(int N = 100)
    {
        segmentCount = (points.Length - 1);
        segments = new Line[segmentCount];
        lengths = new float[segmentCount];

        Length = 0.0f;
        for (var i = 0; i < segmentCount; i++)
        {
            segments[i] = new Line(points[i], points[i + 1]);
            Length += segments[i].EstimateLength();
            lengths[i] = Length;
        }
    }
}
