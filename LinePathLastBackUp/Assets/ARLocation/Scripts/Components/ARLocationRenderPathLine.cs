using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component renders a LocationPath using a given LineRenderer.
/// </summary>

public class ARLocationRenderPathLine : MonoBehaviour
{
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

    //public void RecvLocation(string RecLoc)
    //{
    //    RecLocation1 = RecLoc;
    //}

    // Use this for initialization
    private void Start () {
        //if (locationPath == null)
        //{
        //    throw new System.Exception("null location path");
        //}
    
        string RecLocation = PlayerPrefs.GetString("LocationPath");

        string word = "|";
        string[] words = RecLocation.Split(new string[] { word }, StringSplitOptions.None);
        int cnt = words.Length;

        pointCount = cnt;
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
        string RecLocation = PlayerPrefs.GetString("LocationPath");

        string word = "|";
        string[] words = RecLocation.Split(new string[] { word }, StringSplitOptions.None);
        int cnt = words.Length;

        string[] val = RecLocation.Split('|');

        for (var i = 0; i < cnt; i++)
        {
            Location location1 = new Location();

            if (i == 0)
            {
                location1.longitude = locationProvider.currentLocation.longitude;
                location1.latitude = locationProvider.currentLocation.latitude;
                location1.altitude = -1.5;
            }
            else if (i == cnt - 1)
            {
                string[] val2 = val[i - 1].Split(',');
                location1.longitude = Convert.ToDouble(val2[0]);
                location1.latitude = Convert.ToDouble(val2[1]);
                location1.altitude = -1.5;
            }
            else
            {
                string[] val2 = val[i - 1].Split(',');
                location1.longitude = Convert.ToDouble(val2[0]);
                location1.latitude = Convert.ToDouble(val2[1]);
                location1.altitude = -1.5;
            }

            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.GetComponent<MeshRenderer>().material.color = Color.blue;
            go.name = "Path Point " + i;

            manager.Add(new ARLocationManagerEntry
            {
                instance = go,
                location = location1,
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
        string RecLocation = PlayerPrefs.GetString("LocationPath");

        string word = "|";
        string[] words = RecLocation.Split(new string[] { word }, StringSplitOptions.None);
        int cnt = words.Length;

        string[] val = RecLocation.Split('|');

        for (var i = 0; i < cnt; i++)
        {
            Location location1 = new Location();

            if (i == 0)
            {
                location1.longitude = locationProvider.currentLocation.longitude;
                location1.latitude = locationProvider.currentLocation.latitude;
                location1.altitude = -1.5;
            }
            else if (i == cnt - 1)
            {
                string[] val2 = val[i - 1].Split(',');
                location1.longitude = Convert.ToDouble(val2[0]);
                location1.latitude = Convert.ToDouble(val2[1]);
                location1.altitude = -1.5;
            }
            else
            {
                string[] val2 = val[i - 1].Split(',');
                location1.longitude = Convert.ToDouble(val2[0]);
                location1.latitude = Convert.ToDouble(val2[1]);
                location1.altitude = -1.5;
            }

            var loc = location1;
            points[i] = Camera.main.transform.position + Location.VectorFromTo(location, loc, heightRelativeToDevice).toVector3()
                + new Vector3(0, heightRelativeToDevice ? ((float)loc.altitude) : 0, 0);
        }

        spline = Utils.BuildSpline(SplineType.LinearSpline, points, splineSampleCount, 2.0f);
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
