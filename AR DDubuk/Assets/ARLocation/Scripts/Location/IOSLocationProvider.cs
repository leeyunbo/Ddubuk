using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS && ARGPS_USE_NATIVE_LOCATION
public class IOSLocationProvider : AbstractLocationProvider
{
    public override string name
    {
        get
        {
            return "IOSLocationProvider";
        }
    }

    public override bool isCompassEnabled
    {
        get
        {
            return NativeLocation.GetHeadingAvailable();
        }
    }

    private bool hasRequested = false;

    protected override HeadingReading? ReadHeading()
    {
        double heading = NativeLocation.GetCurrentHeading();
        double magneticHeading = NativeLocation.GetCurrentMagneticHeading();
        double accuracy = NativeLocation.GetCurrentHeadingAccuracy();
        long timestamp = (long)(NativeLocation.GetCurrentHeadingTimestamp() * 1000.0);

        return new HeadingReading()
        {
            heading = FixHeadingDeviceOrientation(heading),
            magneticHeading = FixHeadingDeviceOrientation(magneticHeading),
            accuracy = accuracy,
            isMagneticHeadingAvailable = isCompassEnabled,
            timestamp = timestamp
        };
    }

    protected override LocationReading? ReadLocation()
    {
        var latitude = NativeLocation.GetCurrentLatitude();
        var longitude = NativeLocation.GetCurrentLongitude();
        var altitude = NativeLocation.GetCurrentAltitude();
        var accuracy = NativeLocation.GetCurrentHorizontalAccuracy();
        var floor = NativeLocation.GetCurrentFloorLevel();
        var timestamp = (long) (NativeLocation.GetCurrentTimestamp() * 1000.0);

        return new LocationReading()
        {
            latitude = latitude,
            longitude = longitude,
            altitude = altitude,
            accuracy = accuracy,
            floor = floor,
            timestamp = timestamp
        };
    }

    protected override void RequestLocationAndCompassUpdates()
    {
        NativeLocation.Start(options.distanceFilter);
        hasRequested = true;
    }

    protected override void UpdateLocationRequestStatus()
    {
        bool failed = NativeLocation.GetFailed();
        var _isEnabled = NativeLocation.GetIsEnabled();
        var isLocationEnabled = NativeLocation.GetLocationServicesEnabled();

        if (!_isEnabled && !hasRequested)
        {
            status = LocationProviderStatus.Idle;
            return;
        }

        if (!_isEnabled && hasRequested && !failed)
        {
            status = LocationProviderStatus.Initializing;
            return;
        }

        if (failed && !_isEnabled)
        {
            status = LocationProviderStatus.Failed;
            return;
        }

        status = LocationProviderStatus.Started;
    }

    private double FixHeadingDeviceOrientation(double heading)
    {
        var orientation = Input.deviceOrientation;

        switch (orientation)
        {
            case DeviceOrientation.LandscapeLeft:
                heading += 90.0;
                break;
            case DeviceOrientation.LandscapeRight:
                heading -= 90.0;
                break;
            default:
                break;
        }

        return heading;
    }
}
#else
public class IOSLocationProvider { }
#endif