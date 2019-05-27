using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Apply to a GameObject to place it at a specified geographic location.
/// </summary>
public class ARLocationPlaceAtLocation : MonoBehaviour {
    /// <summary>
    /// The location to place the GameObject at.
    /// </summary>
    [Tooltip("The location to place the GameObject at.")]
    public Location location;

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
    [Tooltip("The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled."), Range(0, 500)]
    public float movementSmoothingFactor = 250.0f;

    [System.NonSerialized, HideInInspector]
    public ARLocationManager manager;

    [System.NonSerialized, HideInInspector]
    private ARLocationManagerEntry entry;

    // Use this for initialization
    void Start () {
        Location currentLoc = new Location();
        currentLoc.longitude = Convert.ToDouble(Input.location.lastData.longitude);
        currentLoc.latitude = Convert.ToDouble(Input.location.lastData.latitude);
        currentLoc.altitude = 0;

        manager = ARLocationManager.Instance;

        entry = new ARLocationManagerEntry
        {
            instance = gameObject,
            location = currentLoc,
            options = new ARLocationObjectOptions
            {
                isHeightRelative = isHeightRelative,
                showDebugInfoPanel = showDebugInfoPanel,
                movementSmoothingFactor = movementSmoothingFactor,
                createInstance = false
            }
        };

        manager.Add(entry);
	}

    /// <summary>
    /// Sets the GameObject's location to a new one.
    /// </summary>
    /// <param name="newLocation"></param>
    public void SetLocation(Location newLocation)
    {
        location = newLocation.Clone();
        entry.location = newLocation.Clone();
        entry.isDirty = true;
    }
}
