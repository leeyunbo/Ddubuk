using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARLocationProvider : Singleton<ARLocationProvider> {

    public ILocationProvider provider { get; private set; }

    [Tooltip("The options for the Location Provider.")]
    public LocationProviderOptions LocationOptions;

    [Tooltip("A mock location for use inside the editor.")]
    public Location MockLocation;

    [Tooltip("The maximum wait time to wait for location initialization.")]
    public uint MaxWaitTime = 20;

    public bool isEnabled
    {
        get
        {
            return provider.isEnabled;
        }
    }

  
    public LocationReading currentLocation
    {
        get
        {
            return provider.currentLocation;
        }
    }

    public LocationReading lastLocation
    {
        get
        {
            return provider.lastLocation;
        }
    }

    public DVector3 currentDisplacement
    {
        get
        {
            return provider.currentDisplacement;
        }
    }

    public HeadingReading currentHeading
    {
        get
        {
            return provider.currentHeading;
        }
    }


    public float TimeSinceStart
    {
        get
        {
            return Time.time - provider.startTime;
        }
    }

    public double distanceFromStartPoint
    {
        get
        {
            return provider.distanceFromStartPoint;
        }
    }

    event LocationUpdatedDelegate onLocationUpdated;
    event CompassUpdateDelegate onCompassUpdated;

    public void Awake()
    {
#if !ARGPS_USE_NATIVE_LOCATION || UNITY_EDITOR
        provider = new UnityLocationProvider();
#endif
#if UNITY_ANDROID && ARGPS_USE_NATIVE_LOCATION && !UNITY_EDITOR
        provider = new AndroidLocationProvider();
#endif
#if UNITY_IOS && ARGPS_USE_NATIVE_LOCATION && !UNITY_EDITOR
        provider = new IOSLocationProvider();
#endif
#if UNITY_EDITOR
        provider = new MockLocationProvider();
        (provider as MockLocationProvider).mockLocation = MockLocation;
#endif

        Debug.Log("[ARLocationProvider]: Using provider " + provider.name);

        provider.options = LocationOptions;

        provider.LocationUpdated += Provider_LocationUpdated;
        provider.CompassUpdated += Provider_CompassUpdated;
    }

    IEnumerator Start () {
        yield return StartCoroutine(provider.Start(MaxWaitTime));
    }

    private void Provider_CompassUpdated(HeadingReading heading, HeadingReading lastReading)
    {
        if (onCompassUpdated != null)
        {
            onCompassUpdated(heading, lastReading);
        }
    }

    private void Provider_LocationUpdated(LocationReading currentLocation, LocationReading lastLocation, DVector3 displacement)
    {
        if (onLocationUpdated != null)
        {
            onLocationUpdated(currentLocation, lastLocation, displacement);
        }
    }

    void Update () {
		if (provider == null || !provider.isEnabled)
        {
            return;
        }

        provider.Update();
	}

    public void OnLocationUpdated(LocationUpdatedDelegate locationUpdatedDelegate)
    {
        onLocationUpdated += locationUpdatedDelegate;
    }

    public void OnCompassUpdated(CompassUpdateDelegate compassUpdateDelegate)
    {
        onCompassUpdated += compassUpdateDelegate;
    }

    /// <summary>
    /// Register a delegate for when the provider enables location updates.
    /// </summary>
    /// <param name="del">Del.</param>
    public void OnEnabled(LocationEnabledDelegate del)
    {
        provider.OnEnabled(del);
    }

    /// <summary>
    /// Register a delegate for when the provider fails to initialize location services.
    /// </summary>
    /// <param name="del">Del.</param>
    public void OnFailed(LocationFailedDelegate del)
    {
        provider.OnFail(del);
    }
}
