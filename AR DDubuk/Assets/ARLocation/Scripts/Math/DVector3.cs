using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = System.Math;

[System.Serializable]
public struct DVector3
{
    public double x;
    public double y;
    public double z;

    /// <summary>
    /// Gets the magnitude of the vector.
    /// </summary>
    /// <value>The magnitude.</value>
    public double magnitude
    {
        get
        {
            return M.Sqrt(x * x + y * y + z * z);
        }
    }

    /// <summary>
    /// Gets the normalized version of this vector.
    /// </summary>
    /// <value>The normalized.</value>
    public DVector3 normalized
    {
        get
        {
            var m = magnitude;

            if (m < 0.00000001)
            {
                return new DVector3(0, 0, 0);
            }

            return new DVector3(x, y, z) / magnitude;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:DVector3"/> struct.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public DVector3(double x = 0.0, double y = 0.0, double z = 0.0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Converts to a Vector3.
    /// </summary>
    /// <returns>The vector2.</returns>
    public Vector3 toVector3()
    {
        return new Vector3((float)x, (float)y, (float)z);
    }

    /// <summary>
    /// Equals the specified v and e.
    /// </summary>
    /// <returns>The equals.</returns>
    /// <param name="v">V.</param>
    /// <param name="e">E.</param>
    public bool Equals(DVector3 v, double e = 0.00005)
    {
        return (M.Abs(x - v.x) <= e) && (M.Abs(y - v.y) <= e) && (M.Abs(z - v.z) <= e);
    }

    /// <summary>
    /// Normalize this instance.
    /// </summary>
    public void Normalize()
    {
        double m = magnitude;
        x /= m;
        y /= m;
        z /= m;
    }

    /// <summary>
    /// Set the specified x and y.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public void Set(double x = 0.0, double y = 0.0, double z = 0.0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:DVector3"/>.
    /// </summary>
    /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:DVector3"/>.</returns>
    override public string ToString()
    {
        return "(" + x + ", " + y + ", " + z + ")";
    }

    /// <summary>
    /// Dot the specified a and b.
    /// </summary>
    /// <returns>The dot.</returns>
    /// <param name="a">The alpha component.</param>
    /// <param name="b">The blue component.</param>
    public static double Dot(DVector3 a, DVector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    /// <summary>
    /// Distance the specified a and b.
    /// </summary>
    /// <returns>The distance.</returns>
    /// <param name="a">The alpha component.</param>
    /// <param name="b">The blue component.</param>
    public static double Distance(DVector3 a, DVector3 b)
    {
        return M.Sqrt(a.x * b.x + a.y * b.y + a.z * b.z);
    }

    /// <summary>
    /// Lerp the specified a, b and t.
    /// </summary>
    /// <returns>The lerp.</returns>
    /// <param name="a">The alpha component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="t">T.</param>
    public static DVector3 Lerp(DVector3 a, DVector3 b, double t)
    {
        double s = M.Max(0, M.Min(t, 1));
        return a * (1 - s) + b * s;
    }

    /// <summary>
    /// Computes the product of <c>a</c> and <c>b</c>, yielding a new <see cref="T:DVector3"/>.
    /// </summary>
    /// <param name="a">The <see cref="DVector3"/> to multiply.</param>
    /// <param name="b">The <see cref="double"/> to multiply.</param>
    /// <returns>The <see cref="T:DVector3"/> that is the <c>a</c> * <c>b</c>.</returns>
    public static DVector3 operator *(DVector3 a, double b)
    {
        return new DVector3(
            b * a.x,
            b * a.y,
            b * a.z
        );
    }

    /// <summary>
    /// Computes the division of <c>a</c> and <c>b</c>, yielding a new <see cref="T:DVector3"/>.
    /// </summary>
    /// <param name="a">The <see cref="DVector3"/> to divide (the divident).</param>
    /// <param name="b">The <see cref="double"/> to divide (the divisor).</param>
    /// <returns>The <see cref="T:DVector3"/> that is the <c>a</c> / <c>b</c>.</returns>
    public static DVector3 operator /(DVector3 a, double b)
    {
        return new DVector3(
            a.x / b,
            a.y / b,
            a.z / b
        );
    }

    /// <summary>
    /// Adds a <see cref="DVector3"/> to a <see cref="DVector3"/>, yielding a new <see cref="T:DVector3"/>.
    /// </summary>
    /// <param name="a">The first <see cref="DVector3"/> to add.</param>
    /// <param name="b">The second <see cref="DVector3"/> to add.</param>
    /// <returns>The <see cref="T:DVector3"/> that is the sum of the values of <c>a</c> and <c>b</c>.</returns>
    public static DVector3 operator +(DVector3 a, DVector3 b)
    {
        return new DVector3(
            a.x + b.x,
            a.y + b.y,
            a.z + b.z
        );
    }

    /// <summary>
    /// Subtracts a <see cref="DVector3"/> from a <see cref="DVector3"/>, yielding a new <see cref="T:DVector3"/>.
    /// </summary>
    /// <param name="a">The <see cref="DVector3"/> to subtract from (the minuend).</param>
    /// <param name="b">The <see cref="DVector3"/> to subtract (the subtrahend).</param>
    /// <returns>The <see cref="T:DVector3"/> that is the <c>a</c> minus <c>b</c>.</returns>
    public static DVector3 operator -(DVector3 a, DVector3 b)
    {
        return new DVector3(
            a.x - b.x,
            a.y - b.y,
            a.z + b.z
        );
    }
}
