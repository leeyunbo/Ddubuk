using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LocationProviderOptions
{
    /// <summary>
    /// The minimum desired update time, in seconds.
    /// </summary>
    [Tooltip("The minimum desired update time, in seconds.")]
    public float updateTime;

    /// <summary>
    /// The minimum distance between consecutive location updates, in meters.
    /// </summary>
    [Tooltip("The minimum distance between consecutive location updates, in meters.")]
    public double distanceFilter;

    /// <summary>
    /// The maximum distance between consectutive location updates, in meters.
    /// </summary>
    [Tooltip("The maximum distance between consectutive location updates, in meters.")]
    public double maxDistanceFilter;

    /// <summary>
    /// The minimum accuracy of accepted location measurements, in meters.
    /// </summary>
    [Tooltip("The minimum accuracy of accepted location measurements, in meters. " +
        "Accuracy here means the radius of uncertainty of the device's location, " +
        "defining a circle where it can possibly be found in.")]
    public double accuracyFilter;

    /// <summary>
    /// The minimum angular change between consecutive heading measurement updates.
    /// </summary>
    [Tooltip("The minimum angular accuracy.")]
    public double headingFilter;

    [Tooltip("At startup, use raw data (unfiltered) for this amount of time.")]
    public float rawTime;
}

public enum LocationProviderStatus
{
    Idle,
    Initializing,
    Started,
    Failed
}

// Location provider delegates/events
public delegate void LocationUpdatedDelegate(LocationReading currentLocation, LocationReading lastLocation, DVector3 displacement);
public delegate void CompassUpdateDelegate(HeadingReading heading, HeadingReading lastReading);
public delegate void LocationEnabledDelegate();
public delegate void LocationFailedDelegate(string message);

public interface ILocationProvider
{
    string name { get; }

    LocationProviderOptions options { get; set; }

    LocationReading currentLocation { get; }
    LocationReading lastLocation { get; }

    DVector3 currentDisplacement { get; }

    HeadingReading currentHeading { get; }
    HeadingReading lastHeading { get; }

    float startTime { get; }
    bool isCompassEnabled { get; }    
    double distanceFromStartPoint { get; }
    bool isEnabled { get; }
    LocationReading startPoint { get; }
    bool isRawTime();

    event LocationUpdatedDelegate LocationUpdated;
    event CompassUpdateDelegate CompassUpdated;
    event LocationEnabledDelegate LocationEnabled;
    event LocationFailedDelegate LocationFailed;

    IEnumerator Start(uint maxWaitTime = 10000);

    void ResetStartPoint();
    void Update();

    void OnEnabled(LocationEnabledDelegate del);
    void OnFail(LocationFailedDelegate del);

    string GetInfoString();
    string GetStatusString();
}