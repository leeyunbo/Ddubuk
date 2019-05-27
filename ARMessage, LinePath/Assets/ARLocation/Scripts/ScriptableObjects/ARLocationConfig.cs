using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This scriptable object holds the global configuration data for the AR + GPS
/// Location plugin.
/// </summary>
[CreateAssetMenu(fileName = "ARLocationConfig", menuName = "ARLocation/ARLocationConfig")]
public class ARLocationConfig : ScriptableObject {

    public static string Version
    {
        get
        {
            return "v2.3.0";
        }
    }

    public enum ARLocationDistanceFunc {
        Haversine,
        PlaneSpherical,
        PlaneEllipsoidalFCC
    };

    [Tooltip("The earth radius, in kilometers, to be used in distance calculations.")]
    public double EarthRadiusInKM = 6372.8;

    [Tooltip("The distance function used to calculate geographical distances.")]
    public ARLocationDistanceFunc DistanceFunction = ARLocationDistanceFunc.Haversine;

    [Tooltip("If true, use a native location module instead of Unity's builtin location services.")]
    public bool UseNativeLocationModule = true;

    [Tooltip("If true, use Vuforia instead of ARFoundation.")]
    public bool UseVuforia = false;
}
