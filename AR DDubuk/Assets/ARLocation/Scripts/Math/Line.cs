using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : Curve
{
    private Vector3 p0;
    private Vector3 p1;

    private float distance;

    public Vector3 P0
    {
        get
        {
            return p0;
        }
        set
        {
            p0 = value;
            Calculate();
        }
    }

    public Vector3 P1
    {
        get
        {
            return p1;
        }
        set
        {
            p1 = value;
            Calculate();
        }
    }

    public Line(Vector3 p0, Vector3 p1)
    {
        this.p0 = p0;
        this.p1 = p1;

        Calculate();
    }

    public void Calculate()
    {
        this.distance = Vector3.Distance(this.p1, this.p0);
    }

    public override float EstimateLength(int N = 100)
    {
        return this.distance;
    }

    public override float GetParameterForLength(float s)
    {
        return s / distance;
    }

    public override Vector3 GetPoint(float u)
    {
        return this.p0 * (1 - u) + this.p1 * u;
    }

    public Vector3 GetTangent(float u)
    {
        return (this.p1 - this.p0) / distance;
    }

    public override CurvePointData GetPointAndTangent(float u)
    {
        return new CurvePointData
        {
            point = GetPoint(u),
            tangent = GetTangent(u)
        };
    }

    public override CurvePointData GetPointAndTangentAtLength(float s)
    {
        var u = GetParameterForLength(s);

        return this.GetPointAndTangent(u);
    }

    public override Vector3 GetPointAtLength(float s)
    {
        var u = GetParameterForLength(s);

        return this.GetPoint(u);
    }

    public override Vector3[] Sample(int N)
    {
        if (N == 0)
        {
            return new Vector3[] { GetPoint(0), GetPoint(1) };
        }

        var sample = new Vector3[N + 2];
        var delta = 1.0f / (N + 1.0f);

        for (var i = 0; i < (N + 2); i++)
        {
            sample[i] = GetPoint(i * delta);
        }

        return sample;
    }
}
