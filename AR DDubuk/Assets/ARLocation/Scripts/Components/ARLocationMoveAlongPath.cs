using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component, when attached to a GameObject, makes it traverse a
/// path that interpolates a given set of geographical locations.
/// </summary>
public class ARLocationMoveAlongPath : MonoBehaviour
{
    /// <summary>
    /// The LocationPath describing the path to be traversed.
    /// </summary>
    [Tooltip("The LocationPath describing the path to be traversed.")]
    public LocationPath locationPath;

    /// <summary>
    /// The speed along the path.
    /// </summary>
    [Tooltip("The speed along the path.")]
    public float speed = 1.0f;

    /// <summary>
    /// The up direction to be used for orientation along the path.
    /// </summary>
    [Tooltip("The up direction to be used for orientation along the path.")]
    public Vector3 up = Vector3.up;

    /// <summary>
    /// If true, play the path traversal in a loop.
    /// </summary>
    [Tooltip("If true, play the path traversal in a loop.")]
    public bool loop = true;

    /// <summary>
    /// If true, start playing automatically.
    /// </summary>
    [Tooltip("If true, start playing automatically.")]
    public bool autoPlay = true;

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
    /// The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled.
    /// </summary>
    [Tooltip("The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled."), Range(0, 500)]
    public float movementSmoothingFactor = 50.0f;

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

    bool playing = false;

    Vector3 translation = new Vector3();

    GameObject debugInfoPanel;
 
    void Start()
    {
        if (locationPath == null)
        {
            throw new Exception("null location path");
        }

        pointCount = locationPath.locations.Length;
        points = new Vector3[pointCount];

        locationProvider = ARLocationProvider.Instance;
        locationProvider.OnLocationUpdated(LocationUpdated);

        manager = ARLocationManager.Instance;
        manager.OnRestart(OnRestartHandler);

        arLocationRoot = Utils.FindAndLogError("ARLocationRoot", "[ARLocationMoveAlongPath]: ARLocationRoot GameObject not found.");

        // Check if we use smooth movement
        if (movementSmoothingFactor > 0)
        {
            gameObject.AddComponent<SmoothMove>();
            GetComponent<SmoothMove>().smoothing = movementSmoothingFactor;
        }

        transform.SetParent(arLocationRoot.transform);

        playing = autoPlay;

        if (showDebugInfo)
        {
            arLocationDebugInfo = Utils.FindAndGetComponent<ARLocationDebugInfo>("ARLocationRoot");

            if (arLocationDebugInfo == null)
            {
                showDebugInfo = false;
            }
            else if (lineRenderer != null)
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

        var prefab = arLocationDebugInfo.debugInfoPrefab;
        var canvas = arLocationDebugInfo.canvas;

        debugInfoPanel = Instantiate(prefab, canvas.transform);
    }

    /// <summary>
    /// Starts playing or resumes the playback.
    /// </summary>
    public void Play()
    {
        playing = true;
    }

    /// <summary>
    /// Moves the object to the spline point corresponding 
    /// to the given parameter.
    /// </summary>
    /// <param name="t">Between 0 and 1</param>
    public void GoTo(float t)
    {
        u = Mathf.Clamp(t, 0, 1);
    }

    /// <summary>
    /// Pauses the movement along the path.
    /// </summary>
    public void Pause()
    {
        playing = false;
    }

    /// <summary>
    /// Stops the movement along the path.
    /// </summary>
    public void Stop()
    {
        playing = false;
        u = 0;
    }

    private void OnRestartHandler()
    {
        isDirty = true;
    }

    void BuildSlpine(Location location) {
        for (var i = 0; i < pointCount; i++)
        {
            var loc = locationPath.locations[i];
            points[i] = Camera.main.transform.position + Location.VectorFromTo(location, loc, heightRelativeToDevice).toVector3()
                + new Vector3(0, heightRelativeToDevice ? ((float)loc.altitude) : 0, 0);
        }

        spline = Utils.BuildSpline(locationPath.splineType, points, splineSampleCount, locationPath.alpha);
        isDirty = false;
    }

    private void LocationUpdated(LocationReading location, LocationReading _, DVector3 delta)
    {
        if (isDirty)
        {
            BuildSlpine(location.ToLocation());
            translation = new Vector3(0, 0, 0);
            return;
        }

        translation +=  heightRelativeToDevice ? new Vector3((float) delta.x, 0, (float) delta.y) : delta.toVector3();        
    }

    /// <summary>
    /// Normalized spline parameter
    /// </summary>
    float u = 0.0f;
    private ARLocationDebugInfo arLocationDebugInfo;
    private GameObject arLocationRoot;

    private void Update()
    {
        if (!playing)
        {
            return;
        }

        // If there is no location provider, or spline, do nothing
        if (locationProvider == null || spline == null || !locationProvider.isEnabled) {
            return;
        }

        // Get spline point at current parameter
        var s = spline.Length * u;

        var data = spline.GetPointAndTangentAtArcLength(s);
        var tan = data.tangent;

        // Move object to the point
        var smoothMove = GetComponent<SmoothMove>();
        var useSmoothMove = !(smoothMove == null || locationProvider.provider.isRawTime());

        if (useSmoothMove)
        {
            smoothMove.Target = data.point - translation;
        }
        else
        {
            transform.localPosition = data.point - translation;
        }

        // Set orientation
        transform.localRotation = Quaternion.LookRotation(new Vector3(tan.x, tan.y, tan.z), up);

        // Check if we reached the end of the spline
        u = u + (speed * Time.deltaTime) / spline.Length;
        if (u >= 1 && !loop)
        {
            u = 0;
            playing = false;
        }
        else
        {
            u = u % 1.0f;
        }

        // If there is a line renderer, render the path
        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true;
            var t = arLocationRoot.GetComponent<Transform>();
            spline.DrawCurveWithLineRenderer(lineRenderer, p => t.TransformVector(p - translation));
        }

        if (showDebugInfo)
        {
            var text = name + "\n"
            + "POS: " + transform.position + "\n"
            + "LPOS: " + transform.localPosition + "\n"
            + "PathLength: " + s + "\n"
            + "PathParam: " + u + "\n"
            + "DST: " + Vector3.Distance(transform.position, Camera.main.transform.position);

            ARLocationDebugInfo.UpdateDebugInfoPanelScreenPositionAndText(gameObject, debugInfoPanel, text);
        }   
    }

   
}
