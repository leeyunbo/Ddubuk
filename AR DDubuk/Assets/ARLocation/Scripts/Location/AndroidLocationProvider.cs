using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID && ARGPS_USE_NATIVE_LOCATION
public class AndroidLocationProvider : AbstractLocationProvider
{
    public override string name { get { return "AndroidLocationProvider"; } }

    private AndroidJavaObject nativeLocationManager;
    private bool hasRequested = false;

    public override bool isCompassEnabled
    {
        get
        {
            return nativeLocationManager.Call<bool>("hasHeading");
        }
    }

    protected override HeadingReading? ReadHeading()
    {
        double heading = nativeLocationManager.Call<float>("getCurrentTrueHeading");
        bool isMagneticHeadingAvailable = nativeLocationManager.Call<bool>("hasHeading");
        long timestamp = nativeLocationManager.Call<long>("getCurrentTimestamp");
        return new HeadingReading() {
            heading = heading,
            isMagneticHeadingAvailable = isMagneticHeadingAvailable,
            timestamp = timestamp
        };
    }

    protected override LocationReading? ReadLocation()
    {
        double latitude = nativeLocationManager.Call<double>("getCurrentLatitude");
        double longitude = nativeLocationManager.Call<double>("getCurrentLongitude");
        double altitude = nativeLocationManager.Call<double>("getCurrentAltitude");
        double accuracy = nativeLocationManager.Call<float>("getHorizontalAccuracy");
        long timestamp = nativeLocationManager.Call<long>("getCurrentTimestamp");

        return new LocationReading()
        {
            latitude = latitude,
            longitude = longitude,
            altitude = altitude,
            accuracy = accuracy,
            timestamp = timestamp
        };
    }

    protected override void RequestLocationAndCompassUpdates()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

        nativeLocationManager = new AndroidJavaObject("com.argps.natlocs.NativeLocationManager", activity, (long) options.updateTime * 1000);
        nativeLocationManager.Call("startNativeLocationService");
        hasRequested = true;
    }

    protected override void UpdateLocationRequestStatus()
    {
        var isEnabled = nativeLocationManager.Call<bool>("isEnabled");

        if (!isEnabled && !hasRequested)
        {
            status = LocationProviderStatus.Idle;
        }

        if (!isEnabled && hasRequested)
        {
            status = LocationProviderStatus.Initializing;
        }

        status = LocationProviderStatus.Started;
    }
}
#else
public class AndroidLocationProvider { }
#endif
