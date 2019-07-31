// Copyright (C) 2018 Daniel Fortes <daniel.fortes@gmail.com>
// All rights reserved.
//
// See LICENSE.TXT for more info


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

#if !ARGPS_USE_VUFORIA
using UnityEngine.XR.ARFoundation;
#endif

/// <summary>
/// The options passed to the ARLocationManager when adding a new positioned GameObject.
/// </summary>
[System.Serializable]
public class ARLocationObjectOptions
{
    /// <summary>
    /// If true, the altitude will be computed as relative to the device level.
    /// </summary>
    [Tooltip("If true, the altitude will be computed as relative to the device level.")]
    public bool isHeightRelative;

    /// <summary>
    /// If true, will display a UI panel with debug information above the object.
    /// </summary>
    [Tooltip("If true, will display a UI panel with debug information above the object.")]
    public bool showDebugInfoPanel;

    /// <summary>
    /// If true, will clone the object when placing it on the scene.
    /// </summary>
    [Tooltip("If true, will clone the object when placing it on the scene.")]
    public bool createInstance = true;

    /// <summary>
    /// The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled.
    /// </summary>
    [Tooltip("The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled."), Range(0, 500)]
    public float movementSmoothingFactor;
};

/// <summary>
/// This structure holds all data for a positioned GameObject in the ARLocationManager.
/// </summary>
[System.Serializable]
public class ARLocationManagerEntry
{
    /// <summary>
    /// The GameObject to be placed in the scene.
    /// </summary>
    [Tooltip("The GameObject to be placed in the scene.")]
    public GameObject instance;

    /// <summary>
    /// The GPS/geolocation coordinates.
    /// </summary>
    [Tooltip("The GPS/geolocation coordinates.")]
    public Location location;

    /// <summary>
    /// The placement options.
    /// </summary>
    [Tooltip("The placement options.")]
    public ARLocationObjectOptions options;

    /// <summary>
    /// Dirty location flag.
    /// </summary>
    [HideInInspector]
    public bool isDirty = true;

    /// <summary>
    /// Changes the location of the entry.
    /// </summary>
    /// <param name="newLocation"></param>
    public void Relocate(Location newLocation)
    {
        location = newLocation.Clone();
        isDirty = true;
    }
}

/// <summary>
/// This Component manages all positioned GameObjects, synchronizing their world position in the scene
/// with their geographical coordinates. This is done by calculating their position relative to the device's position.
/// 
/// Should be placed in a GameObject called "ARLocationRoot", whose parent is the "AR Session Origin".
/// </summary>
/// 
public class ARLocationManager : Singleton<ARLocationManager>
{
    /// <summary>
    /// An array describing a set of objects to be placed on the scene via GPS/geolocation
    /// coordinates.
    /// </summary>
    [Tooltip("An array describing a set of objects to be placed on the scene via GPS/geolocation.")]
    public ARLocationManagerEntry[] objects;
    public Toggle toggle;
    /// <summary>
    /// The ar session reset distance.
    /// </summary>
    [Tooltip("Distance that the user can move away from the initial position before the" +
             "AR Session is reset/refreshed. This is useful if the content's alignment " +
             "is large due to true-north error (which increases with distance). " +
             "A zero value means disabled.")]
    public float arSessionResetDistance = 20.0f;

    /// <summary>
    /// A 2D screen-space canvas to fill the screen while the AR Session is being reset (when arSessionResetDistance > 0)
    /// </summary>
    [Tooltip("A 2D screen-space canvas to fill the screen while the AR Session is being reset (when arSessionResetDistance > 0).")]
    public GameObject resetWaitScreen;

    /// <summary>
    /// A delegate that is called when a new object is addded to the manager.
    /// </summary>
    public delegate void OnObjectAddedDelegate(ARLocationManagerEntry entry);

    /// <summary>
    /// A delegate that is called when the manager is restarted.
    /// </summary>
    public delegate void OnRestartDelegate();

    /// <summary>
    /// Called when the manager has started and objects can be added.
    /// </summary>
    public delegate void OnStartDelegate();

    public Dictionary<int, ARLocationManagerEntry> entries = new Dictionary<int, ARLocationManagerEntry>();

    OnObjectAddedDelegate onObjectAddedDelegates;

    OnStartDelegate onStartDelegate;

    OnRestartDelegate onRestartDelegates;

    ARLocationProvider locationProvider;

    private bool hasStarted = false;

    private bool waitingReset = false;

    private GameObject waitScreen;

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;

        // Find the LocationProvider
        locationProvider = ARLocationProvider.Instance;
        if (locationProvider == null)
        {
            Debug.LogError("[ARFoundation+GPSLocation][ARLocatedObjectsManager]: LocationProvider GameObject or Component not found.");
            return;
        }

        // Add the initially set objects
        foreach (var entry in objects)
        {
            Add(entry);
        }

        // Register callback for handling location updates
        locationProvider.OnLocationUpdated(HandleLocationUpdatedDelegate);

        // Register callback for handling new camera frames
#if !ARGPS_USE_VUFORIA
        ARSubsystemManager.cameraFrameReceived += frameHandler;
#endif
        // Cart onStart delegates
        hasStarted = true;
        if (onStartDelegate != null)
        {
            onStartDelegate();
        }
    }

    /// <summary>
    /// Fetches the entry for a given instance id.
    /// </summary>
    /// <param name="id">The transform instance ID</param>
    /// <returns>ARLocationManagerEntry</returns>
    public ARLocationManagerEntry GetEntry(int id)
    {
        return entries[id];
    }

    public void checkToggle()
    {
        if (toggle.isOn)
        {
            foreach (var entry in entries)
            {          
                entry.Value.instance.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 86);
  
            }
            

        }

        if (!toggle.isOn)
        {
            foreach (var entry in entries)
            {
                entry.Value.instance.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            }

        }
    }
    /// <summary>
    /// Ons the object added.
    /// </summary>
    /// <param name="del">Del.</param>
    public void OnObjectAdded(OnObjectAddedDelegate del)
    {
        onObjectAddedDelegates += del;
    }

    /// <summary>
    /// Adds a delegate to be called when the ARLocationManager session is restarted.
    /// </summary>
    /// <param name="del"></param>
    public void OnRestart(OnRestartDelegate del)
    {
        onRestartDelegates += del;
    }

    /// <summary>
    /// Adds a listener for the OnStart event.
    /// </summary>
    /// <param name="del">Del.</param>
    public void OnStart(OnStartDelegate del)
    {
        if (hasStarted)
        {
            del();
        }
        else
        {
            onStartDelegate += del;
        }
    }

    /// <summary>
    /// Registers a new entry in the ARLocationManager.
    /// </summary>
    /// <param name="entry">Entry.</param>
    public int Add(ARLocationManagerEntry entry)
    {
        int id = -1;
        if (entry.options.createInstance)
        {
            var instance = Instantiate(entry.instance, transform);
            entry.instance = instance;
            id = Mathf.Abs(instance.transform.GetInstanceID());
            entries.Add(id, entry);
        }
        else
        {
            entry.instance.transform.SetParent(transform);
            id = entry.instance.transform.GetInstanceID();
            entries.Add(id, entry);
        }

        // Check if we use smooth movement
        if (entry.options.movementSmoothingFactor > 0)
        {
            entry.instance.AddComponent<SmoothMove>();
            entry.instance.GetComponent<SmoothMove>().smoothing = entry.options.movementSmoothingFactor;
        }

        if (onObjectAddedDelegates != null)
        {
            onObjectAddedDelegates(entry);
        }

        entry.isDirty = true;

        return id;
    }

    /// <summary>
    /// Called when the device location is updated
    /// </summary>
    /// <param name="location">Location.</param>
    /// <param name="accuracy">Accuracy.</param>
    void HandleLocationUpdatedDelegate(LocationReading currentLocation, LocationReading lastLocation, DVector3 displacement)
    {
        UpdatePositions(currentLocation.ToLocation(), displacement);
    }

    /// <summary>
    /// Updates the position of all the GameObjects
    /// </summary>
    /// <param name="deviceLocation">Location.</param>
    void UpdatePositions(Location deviceLocation, DVector3 delta)
    {
        foreach (var entry in entries)
        {
            UpdateObjectPosition(entry.Value, deviceLocation, delta);
        }
    }

    /// <summary>
    /// Updates the object position.
    /// </summary>
    /// <param name="id">The object's transform instance ID.</param>
    public void UpdateObjectPosition(int id)
    {
        var entry = GetEntry(id);

        if (entry == null)
        {
            return;
        }

        var currentLocation = locationProvider.currentLocation;
        var currentDelta = locationProvider.currentDisplacement;

        UpdateObjectPosition(entry, currentLocation.ToLocation(), currentDelta, false);
    }

    /// <summary>
    /// Updates the object position.
    /// </summary>
    /// <param name="instance">Instance.</param>
    /// <param name="instanceLocation">Instance location.</param>
    /// <param name="instanceOptions">Instance options.</param>
    /// <param name="deviceLocation">Device location.</param>
    public void UpdateObjectPosition(
        ARLocationManagerEntry entry,
        Location deviceLocation,
        DVector3 delta,
        bool forceDisableSmooth = false
    )
    {
        var instance = entry.instance;
        var instanceLocation = entry.location;
        var instanceOptions = entry.options;
        var smoothMove = instance.GetComponent<SmoothMove>();

        var useSmoothMove = !(smoothMove == null || forceDisableSmooth || locationProvider.provider.isRawTime());


        if (useSmoothMove)
        {
            smoothMove.Target = Location.GetGameObjectPositionForLocation(
                Camera.main.transform, deviceLocation, instanceLocation, instanceOptions.isHeightRelative
            );
        }
        else
        {
            Location.PlaceGameObjectAtLocation(
                instance.transform, Camera.main.transform, deviceLocation, instanceLocation, instanceOptions.isHeightRelative
            );
        }

    }

    /// <summary>
    /// Handler called when new cameraa frame arrives.
    /// </summary>
    /// <param name="obj">Object.</param>
#if !ARGPS_USE_VUFORIA
    private void frameHandler(ARCameraFrameEventArgs obj)
    {
        if (!waitingReset)
        {
            return;
        }

        if (waitScreen != null)
        {
            // Flag all objects as diry
            foreach (var item in entries)
            {
                item.Value.isDirty = true;
            }

            // Call restart callbacks
            if (onRestartDelegates != null)
            {
                onRestartDelegates();
            }

            // Remove the wait screen.
            Destroy(waitScreen);
        }

        waitingReset = false;
    }
#endif

    /// <summary>
    /// Restarts the current ARLocationManager session. Causes the ARFoundation session
    /// to reset, and flags all objects as dirty.
    /// </summary>
    private void Restart()
    {
#if !ARGPS_USE_VUFORIA
        var arSession = GameObject.Find("AR Session").GetComponent<ARSession>();
        waitScreen = Instantiate(resetWaitScreen, Camera.main.transform);
        waitingReset = true;

        arSession.Reset();

        var orientation = GetComponent<ARLocationOrientation>();

        if (orientation != null)
        {
            orientation.Restart();
        }

        if (onRestartDelegates != null)
        {
            onRestartDelegates();
        }
#endif
    }

    void Update()
    {
        // Reset the AR Session if: arSessionReset > 0 and the user has walked arSessionResetDistance from
        // the current session origin.
        if ((arSessionResetDistance > 0) && (Camera.main.transform.localPosition.magnitude > arSessionResetDistance))
        {
#if !ARGPS_USE_VUFORIA
            if (ARSubsystemManager.systemState == ARSystemState.None || ARSubsystemManager.systemState == ARSystemState.Unsupported)
            {
                return;
            }

            Restart();
#endif
        }
    }
}
