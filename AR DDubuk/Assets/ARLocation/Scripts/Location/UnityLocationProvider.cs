using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = System.Math;

public class UnityLocationProvider : AbstractLocationProvider, ILocationProvider
{
    public override string name { get { return "UnityLocationProvider"; } }

    public override bool isCompassEnabled
    {
        get
        {
            return Input.compass.enabled;
        }
    }

    protected override void RequestLocationAndCompassUpdates()
    {
        Debug.Log("[UnityLocationProvider]: Requesting location updates...");

        Input.compass.enabled = true;

        Input.location.Start(
            (float)options.accuracyFilter,
            (float)options.distanceFilter
        );
    }

    protected override void UpdateLocationRequestStatus()
    {
        switch (Input.location.status)
        {
            case LocationServiceStatus.Initializing:
                status = LocationProviderStatus.Initializing;
                break;

            case LocationServiceStatus.Failed:
                status = LocationProviderStatus.Failed;
                break;

            case LocationServiceStatus.Running:
                status = LocationProviderStatus.Started;
                break;

            case LocationServiceStatus.Stopped:
                status = LocationProviderStatus.Idle;
                break;
        }
    }

    protected override Nullable<LocationReading> ReadLocation()
    {
        if (!isEnabled)
        {
            return null;
        }

        var data = Input.location.lastData;

        return new LocationReading()
        {
            latitude = (double)data.latitude,
            longitude = (double)data.longitude,
            altitude = (double)data.altitude,
            accuracy = (double)data.horizontalAccuracy,
            floor = -1,
            timestamp = (long) (data.timestamp * 1000)
        };
    }

    protected override Nullable<HeadingReading> ReadHeading()
    {
        if (!isEnabled)
        {
            return null;
        }

        return new HeadingReading()
        {
            heading = (double) Input.compass.trueHeading,
            magneticHeading = (double) Input.compass.magneticHeading,
            accuracy = (double) Input.compass.headingAccuracy,
            timestamp = (long) (Input.compass.timestamp * 1000),
            isMagneticHeadingAvailable = Input.compass.enabled
        };
    }
}

