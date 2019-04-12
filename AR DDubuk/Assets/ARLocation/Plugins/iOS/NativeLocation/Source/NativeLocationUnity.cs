using UnityEngine;
using System.Runtime.InteropServices;

public class NativeLocation : MonoBehaviour
{
    #region Declare external C interface    
#if UNITY_IOS && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern void _nl_start(double updateDistance);

    [DllImport("__Internal")]
    private static extern double _nl_get_current_latitude();

    [DllImport("__Internal")]
    private static extern double _nl_get_current_longitude();

    [DllImport("__Internal")]
    private static extern double _nl_get_current_altitude();

    [DllImport("__Internal")]
    private static extern double _nl_get_current_horizontal_accuracy();

    [DllImport("__Internal")]
    private static extern double _nl_get_current_vertical_accuracy();

    [DllImport("__Internal")]
    private static extern double _nl_get_current_timestamp();

    [DllImport("__Internal")]
    private static extern int _nl_get_current_floor_level();

    [DllImport("__Internal")]
    private static extern double _nl_get_current_heading();

    [DllImport("__Internal")]
    private static extern double _nl_get_current_magnetic_heading();

    [DllImport("__Internal")]
    private static extern double _nl_get_current_heading_accuracy();

    [DllImport("__Internal")]
    private static extern double _nl_get_current_heading_timestamp();

    [DllImport("__Internal")]
    private static extern bool _nl_location_services_enabled();

    [DllImport("__Internal")]
    private static extern bool _nl_heading_available();

    [DllImport("__Internal")]
    private static extern bool _nl_get_is_enabled();

    [DllImport("__Internal")]
    private static extern bool _nl_get_failed();



#endif
    #endregion

    #region Wrapped methods and properties
    public static void Start(double updateDistance = 1)
    {
#if UNITY_IOS && !UNITY_EDITOR
       _nl_start(updateDistance);
#endif
    }

    public static double GetCurrentLatitude()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_current_latitude();
#endif
        return -1.0;
    }

    public static double GetCurrentLongitude()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_current_longitude();
#endif
        return -1.0;
    }

    public static double GetCurrentAltitude()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_current_altitude();
#endif
        return -1.0;
    }

    public static double GetCurrentHorizontalAccuracy()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_current_horizontal_accuracy();
#endif
        return -1.0;
    }

    public static double GetCurrentVerticalAccuracy()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_current_vertical_accuracy();
#endif
        return -1.0;
    }

    public static double GetCurrentTimestamp()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_current_timestamp();
#endif
        return -1.0;
    }

    public static int GetCurrentFloorLevel()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_current_floor_level();
#endif
        return 0;
    }

    public static double GetCurrentHeading()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_current_heading();
#endif
        return 0;
    }

    public static double GetCurrentMagneticHeading()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_current_magnetic_heading();
#endif
        return 0;
    }

    public static double GetCurrentHeadingAccuracy()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_current_heading_accuracy();
#endif
        return 0;
    }

    public static double GetCurrentHeadingTimestamp()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_current_heading_timestamp();
#endif
        return 0;
    }

    public static bool GetLocationServicesEnabled()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_location_services_enabled();
#endif
        return false;
    }

    public static bool GetHeadingAvailable()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_heading_available();
#endif
        return false;
    }

    public static bool GetIsEnabled()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_is_enabled();
#endif
        return false;
    }

    public static bool GetFailed()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _nl_get_failed();
#endif
        return false;
    }

    #endregion
}
