using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data used to construct a spline passing trough a set of geographical
/// locations.
/// </summary>
[CreateAssetMenu(fileName = "AR Location Path", menuName = "ARLocation/Path")]
public class LocationPath : ScriptableObject
{
    /// <summary>
    /// The geographical locations that the path will interpolate.
    /// </summary>
    [Tooltip("The geographical locations that the path will interpolate.")]
    public Location[] locations;

    [Tooltip("The type of the spline used")]
    public SplineType splineType = SplineType.CatmullromSpline;

    /// <summary>
    /// The path's alpha/tension factor.
    /// </summary>
    [Tooltip("The path's alpha/tension factor.")]
    public float alpha = 0.5f;

    /// <summary>
    /// The scale used in the editor scene viewer for drawing the path.
    /// </summary>
    public float sceneViewScale = 1.0f;
}
