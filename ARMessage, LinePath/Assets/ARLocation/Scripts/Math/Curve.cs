using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct holding a pair of point/tangent values.
/// </summary>
public struct CurvePointData
{
    public Vector3 point;
    public Vector3 tangent;
}


public abstract class Curve
{
    public abstract Vector3 GetPoint(float u);
   
    public abstract CurvePointData GetPointAndTangent(float u);

    public abstract Vector3[] Sample(int N);

    public abstract float EstimateLength(int N = 100);

    public abstract float GetParameterForLength(float s);

    public abstract Vector3 GetPointAtLength(float s);

    public abstract CurvePointData GetPointAndTangentAtLength(float s);
}
