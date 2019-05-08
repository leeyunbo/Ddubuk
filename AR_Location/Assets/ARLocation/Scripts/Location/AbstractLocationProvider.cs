using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

/// <summary>
/// Abstract location provider. All concrete location provider implementations
/// should derive from this.
/// </summary>
public abstract class AbstractLocationProvider : ILocationProvider
{
    /// <summary>
    /// The name of the location provider.
    /// </summary>
    /// <value>The name.</value>
    public abstract string name { get; }

    /// <summary>
    /// The options of the location provider.
    /// </summary>
    /// <value>The options.</value>
    public LocationProviderOptions options { get; set; }

    /// <summary>
    /// Gets or sets the current location.
    /// </summary>
    /// <value>The current location.</value>
    public LocationReading currentLocation { get; protected set; }

    /// <summary>
    /// Gets or sets the previous location.
    /// </summary>
    /// <value>The last location.</value>
    public LocationReading lastLocation { get; protected set; }

    /// <summary>
    /// Gets or sets the current displacement, i.e., the displacement
    /// from the previous to the current location.
    /// </summary>
    /// <value>The current displacement.</value>
    public DVector3 currentDisplacement { get; protected set; }

    /// <summary>
    /// Gets or sets the current raw location reading.
    /// </summary>
    /// <value>The raw location current.</value>
    public LocationReading rawLocationCurrent { get; protected set; }

    /// <summary>
    /// Gets or sets the previous raw location reading.
    /// </summary>
    /// <value>The raw location last.</value>
    public LocationReading rawLocationLast { get; protected set; }

    /// <summary>
    /// The current heading reading.
    /// </summary>
    /// <value>The current heading.</value>
    public HeadingReading currentHeading { get; protected set; }

    /// <summary>
    /// The previous heading reading.
    /// </summary>
    /// <value>The last heading.</value>
    public HeadingReading lastHeading { get; protected set; }

    /// <summary>
    ///  The start point, i.e., the first measured location.
    /// </summary>
    /// <value>The start point.</value>
    public LocationReading startPoint { get; protected set; }

    /// <summary>
    /// Gets or sets the current status of the location provider.
    /// </summary>
    /// <value>The status.</value>
    public LocationProviderStatus status { get; protected set; }

    /// <summary>
    /// If true, the location provider is enablied and getting regular location
    /// updated from the device.
    /// </summary>
    /// <value><c>true</c> if is enabled; otherwise, <c>false</c>.</value>
    public bool isEnabled { get; protected set; }

    /// <summary>
    /// If true, the first reading has not occured yet.
    /// </summary>
    /// <value><c>true</c> if first reading; otherwise, <c>false</c>.</value>
    public bool firstReading { get; protected set; }

    /// <summary>
    /// If true, the provider has a functioning magnetic compass sensor.
    /// </summary>
    /// <value><c>true</c> if is compass enabled; otherwise, <c>false</c>.</value>
    public abstract bool isCompassEnabled { get; }

    /// <summary>
    /// The start time of the location provider.
    /// </summary>
    /// <value>The start time.</value>
    public float startTime { get; protected set; }
    public double distanceFromStartPoint
    {
        get
        {
            return LocationReading.HorizontalDistance(startPoint, currentLocation);
        }
    }

    /// <summary>
    /// Event for when a new location data is received.
    /// </summary>
    public event LocationUpdatedDelegate LocationUpdated;

    /// <summary>
    /// Event for when a new compass data is received.
    /// </summary>
    public event CompassUpdateDelegate CompassUpdated;
    public event LocationEnabledDelegate LocationEnabled;
    public event LocationFailedDelegate LocationFailed;

    /// <summary>
    /// Reads the location from the device; should be implemented by each
    /// provider.
    /// </summary>
    /// <returns>The location.</returns>
    protected abstract LocationReading? ReadLocation();

    /// <summary>
    /// Reads the heading from the device; should be implemented by each
    /// provider.
    /// </summary>
    /// <returns>The heading.</returns>
    protected abstract HeadingReading? ReadHeading();

    /// <summary>
    /// Requests the location and compass updates from the device; should be implemented by each
    /// provider.
    /// </summary>
    protected abstract void RequestLocationAndCompassUpdates();

    /// <summary>
    /// Updates the location service status from the device; should be implemented by each
    /// provider.
    /// </summary>
    protected abstract void UpdateLocationRequestStatus();

    protected AbstractLocationProvider()
    {
        isEnabled = false;
        firstReading = true;
        status = LocationProviderStatus.Idle;
    }

    public virtual IEnumerator Start(uint maxWaitTime = 10000)
    {
        Debug.Log("[AbstractLocationProvider]: Starting...");

#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        yield return new WaitForSeconds(1);
#endif

        RequestLocationAndCompassUpdates();
        uint maxWait = maxWaitTime;
        UpdateLocationRequestStatus();
        while (status == LocationProviderStatus.Initializing && maxWait > 0)
        {
            Debug.Log("[AbstractLocationProvider]: Wait... " + maxWait);

            yield return new WaitForSeconds(1);
            maxWait--;
            UpdateLocationRequestStatus();
        }

        if (maxWait < 1)
        {
            Debug.LogError("[AbstractLocationProvider]: Timed out.");

            if (LocationFailed != null)
            {
                LocationFailed("Timed out");
            }
            yield break;
        }

        if (status == LocationProviderStatus.Failed)
        {
            Debug.LogError("[AbstractLocationProvider]: Falied to initialize location updates.");

            if (LocationFailed != null)
            {
                LocationFailed("Falied to initialize location updates.");
            }
            yield break;
        }

        if (status != LocationProviderStatus.Started)
        {
            Debug.LogError("[AbstractLocationProvider]: Unknown error initializing location updates. " + status);

            if (LocationFailed != null)
            {
                LocationFailed("Unknown error initializing location updates.");
            }
            yield break;
        }

        Debug.Log("[AbstractLocationProvider]: Started!");

        isEnabled = true;
        firstReading = true;
        startTime = Time.time;

        if (LocationEnabled != null)
        {
            LocationEnabled();
        }
    }


    protected void EmitLocationUpdated()
    {
        if (LocationUpdated != null)
        {
            LocationUpdated(currentLocation, lastLocation, currentDisplacement);
        }
    }

    protected void EmitCompassUpdated()
    {
        if (CompassUpdated != null)
        {
            CompassUpdated(currentHeading, lastHeading);
        }
    }

    protected void UpdateLocation(LocationReading newLocation)
    {
        rawLocationLast = rawLocationCurrent;
        rawLocationCurrent = newLocation;

        if (!ShouldUpdateLocation(newLocation))
        {
            return;
        }

        lastLocation = currentLocation;
        currentLocation = newLocation;

        currentDisplacement = Location.VectorFromTo(lastLocation.ToLocation(), currentLocation.ToLocation());

        EmitLocationUpdated();
    }

    protected void UpdateHeading(HeadingReading newHeading)
    {
        if (!ShouldUpdateHeading(newHeading))
        {
            return;
        }

        lastHeading = currentHeading;
        currentHeading = newHeading;

        EmitCompassUpdated();
    }

    public bool isRawTime()
    {
        return (options.rawTime > 0) && (Time.time - startTime) < options.rawTime;
    }

    protected bool ShouldUpdateHeading(HeadingReading newHeading)
    {
        if (isRawTime() && newHeading.timestamp != currentHeading.timestamp)
        {
            return true;
        }

        if (newHeading.timestamp == currentHeading.timestamp)
        {
            return false;
        }

        if ((newHeading.accuracy > options.headingFilter) && options.headingFilter > 0)
        {
            return false;
        }

        return true;
    }

    protected bool ShouldUpdateLocation(LocationReading newLocation)
    {
        if (isRawTime() && newLocation.timestamp != currentLocation.timestamp)
        {
            return true;
        }
        if ((options.maxDistanceFilter > 0) && (LocationReading.HorizontalDistance(rawLocationLast, rawLocationCurrent) > options.maxDistanceFilter))
        {
            return false;
        }

        if ((newLocation.timestamp == currentLocation.timestamp) || (newLocation.timestamp - currentLocation.timestamp < ((long) (options.updateTime * 1000))))
        {
            return false;
        }

        if (LocationReading.HorizontalDistance(newLocation, currentLocation) < options.distanceFilter)
        {
            return false;
        }

        if ((newLocation.accuracy > options.accuracyFilter) && (options.accuracyFilter > 0))
        {
            return false;
        }


        return true;
    }

    public virtual void Update()
    {
        if (status != LocationProviderStatus.Started || !isEnabled)
        {
            return;
        }

        var location = ReadLocation();
        var heading = ReadHeading();

        if (location == null || heading == null)
        {
            Debug.Log("[AbstractLocationProvider]: Null reading");
            return;
        }

        if (firstReading)
        {
            startPoint = location.Value;
            currentLocation = startPoint;
            currentHeading = heading.Value;
            currentDisplacement = Location.VectorFromTo(lastLocation.ToLocation(), currentLocation.ToLocation());
            firstReading = false;

            EmitCompassUpdated();
            EmitLocationUpdated();

            return;
        }

        UpdateLocation(location.Value);
        UpdateHeading(heading.Value);
    }

    public void ResetStartPoint()
    {
        startPoint = currentLocation;
    }

    public string GetStatusString()
    {
        switch (status)
        {
            case LocationProviderStatus.Idle:
                return "Idle";
            case LocationProviderStatus.Failed:
                return "Failed";
            case LocationProviderStatus.Initializing:
                return "Initializing";
            case LocationProviderStatus.Started:
                return "Started";
        }

        return "UnknownStatus";
    }

    public string GetInfoString()
    {
        return name + 
            "{ \n" + 
            currentLocation + "\n" + 
            currentHeading + "\n" +  
            "Status = " + GetStatusString() + "\n" +
            "DistanceFromStartPoint = " + distanceFromStartPoint + "\n" +
            "TimeSinceStart = " + (Time.time - startTime) + "\n" +
            "}";
    }

    public void OnEnabled(LocationEnabledDelegate del)
    {
        LocationEnabled += del;

        if (isEnabled)
        {
            del();
        }
    }

    public void OnFail(LocationFailedDelegate del)
    {
        LocationFailed += del;
    }
}