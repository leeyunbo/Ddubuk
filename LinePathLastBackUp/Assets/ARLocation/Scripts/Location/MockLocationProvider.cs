using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockLocationProvider : AbstractLocationProvider
{
    public override string name
    {
        get
        {
            return "MockLocationProvider";
        }
    }

    public override bool isCompassEnabled
    {
        get
        {
            return true;
        }
    }

    public Location mockLocation = new Location();

    protected override HeadingReading? ReadHeading()
    {
        var transform = Camera.main.transform;

        return new HeadingReading {
            heading = transform.localEulerAngles.y,
            magneticHeading = transform.localEulerAngles.y,
            accuracy = 0,
            isMagneticHeadingAvailable = true,
            timestamp = (long) (Time.time * 1000)
        };
    }

    protected override LocationReading? ReadLocation()
    {
        return new LocationReading
        {
            latitude = mockLocation.latitude,
            longitude = mockLocation.longitude,
            altitude = mockLocation.altitude,
            accuracy = 0.0,
            floor = -1,
            timestamp = 0
        };
    }

    private bool requested = true;

    protected override void RequestLocationAndCompassUpdates()
    {
        requested = true;
        return;
    }

    protected override void UpdateLocationRequestStatus()
    {
        if (requested)
        {
            status = LocationProviderStatus.Initializing;
            requested = false;
        }

        if (status == LocationProviderStatus.Initializing)
        {
            status = LocationProviderStatus.Started;
        }
    }
}
