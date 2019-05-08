using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class instantiates a prefab at the given GPS locations. Must 
/// be in the `ARLocationRoot` GameObject with a `ARLocatedObjectsManager` 
/// Component.
/// </summary>
public class ARLocationPlaceGameObjectAtLocations : MonoBehaviour
{
    /// <summary>
    /// The locations where the objects will be instantiated.
    /// </summary>
    [Tooltip("The locations where the objects will be instantiated.")]
    public List<Location> locations;

    /// <summary>
    /// The game object that will be instantiated.
    /// </summary>
    [Tooltip("The game object that will be instantiated.")]
    public GameObject prefab;

    /// <summary>
    /// If true, all altitude will be considered relative to the device.
    /// </summary>
    [Tooltip("If true, all altitude will be considered relative to the device.")]
    public bool isHeightRelative = true;

    /// <summary>
    /// If true, when a `ARLocatedObjectsDebugInfo` is present, a UI Panel with debug information will appear on the top of the object.
    /// </summary>
    [Tooltip("If true, when a `ARLocatedObjectsDebugInfo` is present, a UI Panel with debug information will appear on the top" +
             "of the object.")]
    public bool showDebugInfoPanel = true;

    /// <summary>
    /// The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled.
    /// </summary>
    [Tooltip("The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled."), Range(0, 500)]
    public float movementSmoothingFactor = 120f;

    ARLocationManager manager;

    private void Start()
    {
        manager = ARLocationManager.Instance;

        if (manager == null) {
            Debug.LogError("[ARFoundation+GPSLocation][PlaceAtLocations]: ARLocatedObjectsManager Component not found.");
            return;
        }

        locations.ForEach(AddLocation);
    }

    /// <summary>
    /// Adds a location to the locations list.
    /// </summary>
    /// <param name="location"></param>
    public void AddLocation(Location location)
    {
        manager.Add(new ARLocationManagerEntry
        {
            instance = prefab,
            location = location,
            options = new ARLocationObjectOptions {
                isHeightRelative = isHeightRelative,
                showDebugInfoPanel = showDebugInfoPanel,
                movementSmoothingFactor = movementSmoothingFactor,
                createInstance = true,
            }
        });
    }
}