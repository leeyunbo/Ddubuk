using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component renders a LocationPath using a given LineRenderer.
/// </summary>
public class ARLocationRenderPathLine : MonoBehaviour {
    /// <summary>
    /// The LocationPath describing the path to be traversed.
    /// </summary>
    [Tooltip("The LocationPath describing the path to be traversed.")]
    public LocationPath locationPath;

    /// <summary>
    /// If true, all altitude data is considered relative to the current device elevation.
    /// </summary>
    [Tooltip("If true, all altitude data is considered relative to the current device elevation.")]
    public bool heightRelativeToDevice = true;

    /// <summary>
    /// The number of points-per-segment used to calculate the spline.
    /// </summary>
    [Tooltip("The number of points-per-segment used to calculate the spline.")]
    public int splineSampleCount = 250;

    /// <summary>
    /// The number of points-per-segment used to calculate the spline.
    /// </summary>
    [Tooltip("The number of points-per-segment used draw the spline.")]
    public int lineRenderSampleCount = 250;

    /// <summary>
    /// Renders debug information. Requires an ARLocationDebugInfo at the ARLocationRoot object.
    /// </summary>
    [Tooltip("Renders debug information. Requires an ARLocationDebugInfo at the ARLocationRoot object.")]
    public bool showDebugInfo = true;

    /// <summary>
    /// If present, renders the spline in the scene using the given line renderer.
    /// </summary>
    [Tooltip("If present, renders the spline in the scene using the given line renderer.")]
    public LineRenderer lineRenderer;

    ARLocationProvider locationProvider;

    ARLocationManager manager;

    Spline spline;

    Vector3[] points;

    int pointCount;

    bool isDirty = true;

    Vector3 translation = new Vector3();

    private ARLocationDebugInfo arLocationDebugInfo;
    private GameObject arLocationRoot;

    // Use this for initialization
    void Start () {
        if (locationPath == null)
        {
            throw new System.Exception("null location path");
        }

        pointCount = locationPath.locations.Length;
        points = new Vector3[pointCount];

        locationProvider = ARLocationProvider.Instance;
        locationProvider.OnLocationUpdated(LocationUpdated);

        manager = ARLocationManager.Instance;
        manager.OnRestart(OnRestartHandler);

        arLocationRoot = Utils.FindAndLogError("ARLocationRoot", "[ARLocationMoveAlongPath]: ARLocationRoot GameObject not found.");

        transform.SetParent(arLocationRoot.transform);

        if (showDebugInfo)
        {
            arLocationDebugInfo = Utils.FindAndGetComponent<ARLocationDebugInfo>("ARLocationRoot");

            if (arLocationDebugInfo == null)
            {
                showDebugInfo = false;
            }
            else
            {
                SetupDebugObjects();
            }
        }
    }

    /// <summary>
    /// Add control points to the manager for debugging.
    /// </summary>
    void SetupDebugObjects()
    {
        for (var i = 0; i < locationPath.locations.Length; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.GetComponent<MeshRenderer>().material.color = Color.blue;
            go.name = "Path Point " + i;

            manager.Add(new ARLocationManagerEntry
            {
                instance = go,
                location = locationPath.locations[i],
                options = new ARLocationObjectOptions
                {
                    isHeightRelative = heightRelativeToDevice,
                    showDebugInfoPanel = true,
                    createInstance = false,
                    movementSmoothingFactor = 0
                }
            });
        }
    }

    private void OnRestartHandler()
    {
        isDirty = true;
    }

    void BuildSlpine(Location location)
    {
        for (var i = 0; i < pointCount; i++)
        {
            var loc = locationPath.locations[i];
            points[i] = Camera.main.transform.position + Location.VectorFromTo(location, loc, heightRelativeToDevice).toVector3()
                + new Vector3(0, heightRelativeToDevice ? ((float)loc.altitude) : 0, 0);
        }

        spline = Utils.BuildSpline(locationPath.splineType, points, splineSampleCount, locationPath.alpha);
        isDirty = false;
    }

    private void LocationUpdated(LocationReading currentLocation, LocationReading lastLocation, DVector3 displacement)
    {
        if (isDirty)
        {
            BuildSlpine(currentLocation.ToLocation());
            translation = new Vector3(0, 0, 0);
            return;
        }

        translation += heightRelativeToDevice ? new Vector3((float)displacement.x, 0, (float)displacement.y) : displacement.toVector3();
    }


    // Update is called once per frame
    void Update () {
        // If there is no location provider, or spline, do nothing
        if (locationProvider == null || spline == null || !locationProvider.isEnabled)
        {
            return;
        }
        

        // If there is a line renderer, render the path
        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true;
            var t = arLocationRoot.transform;
            if (t != null)
            {
                spline.DrawCurveWithLineRenderer(lineRenderer, p => t.TransformVector(p - translation), lineRenderSampleCount);
            }
        }
    }
}
