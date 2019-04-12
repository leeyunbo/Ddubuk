using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component places instances of a given prefab/GameObject along
/// equally spaced positions in a LocationPath. Should be placed in 
/// the ARLocationRoot GameObject.
/// </summary>
public class ARLocationPlaceGameObjectAlongPath : MonoBehaviour {

    /// <summary>
    /// The path to place the prefab instances on.
    /// </summary>
    [Tooltip("The path to place the prefab instances on.")]
    public LocationPath path;

    /// <summary>
    /// The prefab/GameObject to be palced along the path.
    /// </summary>
    [Tooltip("The prefab/GameObject to be palced along the path.")]
    public GameObject prefab;

    /// <summary>
    /// The number of object instances to be placed, excluding the endpoints. That is, 
    /// the total number of instances is equal to objectCount + 2
    /// </summary>
    [Tooltip("The number of object instances to be placed, excluding the endpoints. That is, the total number of instances is equal to objectCount + 2")]
    public int objectCount = 10;

    /// <summary>
    /// If true, the altitude will be computed as relative to the device level.
    /// </summary>
    [Tooltip("If true, the altitude will be computed as relative to the device level.")]
    public bool isHeightRelative = true;

    /// <summary>
    /// If true, will display a UI panel with debug information above the object.
    /// </summary>
    [Tooltip("If true, will display a UI panel with debug information above the object.")]
    public bool showDebugInfoPanel = false;

    /// <summary>
    /// The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled.
    /// </summary>
    [Tooltip("The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled.")]
    public float movementSmoothingFactor = 0;

    /// <summary>
    /// The size of the sample used to calculate the spline.
    /// </summary>
    [Tooltip("The size of the sample used to calculate the spline.")]
    public int splineSampleSize = 200;

    Spline spline;

    ARLocationManagerEntry[] entries;

    ARLocationManager manager;

    Vector3[] points;

    private void Start()
    {
        manager = ARLocationManager.Instance;

        points = new Vector3[path.locations.Length];

        for (var i = 0; i < points.Length; i++)
        {
            points[i] = path.locations[i].ToVector3();
        }

        spline = Utils.BuildSpline(path.splineType, points, splineSampleSize, path.alpha);

        var sample = spline.SamplePoints(objectCount);
        entries = new ARLocationManagerEntry[sample.Length];

        for (var i = 0; i < entries.Length; i++)
        {
            entries[i] = new ARLocationManagerEntry
            {
                instance = prefab,
                location = new Location(sample[i].z, sample[i].x, sample[i].y),
                options = new ARLocationObjectOptions
                {
                    isHeightRelative = isHeightRelative,
                    movementSmoothingFactor = movementSmoothingFactor,
                    showDebugInfoPanel = showDebugInfoPanel,
                    createInstance = true
                }
            };

            manager.Add(entries[i]);
        }
    }
}
