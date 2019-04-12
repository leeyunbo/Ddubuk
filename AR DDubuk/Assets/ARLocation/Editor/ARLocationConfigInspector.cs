using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Inspector for the ARLocationConfig. This inspector is the main configuration
/// interface for the AR+GPS Location plugin.
/// </summary>
[CustomEditor(typeof(ARLocationConfig))]
public class ARLocationConfigInspector : Editor {

    SerializedProperty p_EarthRadiusInKM;
    SerializedProperty p_DistanceFunction;
    SerializedProperty p_UseNativeLocationModule;
    SerializedProperty p_UseVuforia;

    DefineSymbolsManager defineSymbolsManager;

    const string ARGPS_USE_VUFORIA = "ARGPS_USE_VUFORIA";
    const string ARGPS_USE_NATIVE_LOCATION = "ARGPS_USE_NATIVE_LOCATION";

    Dictionary<string, string> defineSymbolProps = new Dictionary<string, string> {
        {ARGPS_USE_VUFORIA, "UseVuforia"},
        {ARGPS_USE_NATIVE_LOCATION, "UseNativeLocationModule"}
    };

    private string[] distanceFunctionOptions =
    {
        "Haversine",
        "Flat"
    };

    private void OnEnable()
    {
        p_EarthRadiusInKM = serializedObject.FindProperty("EarthRadiusInKM");
        p_DistanceFunction = serializedObject.FindProperty("DistanceFunction");
        p_UseNativeLocationModule = serializedObject.FindProperty("UseNativeLocationModule");
        p_UseVuforia = serializedObject.FindProperty("UseVuforia");

        defineSymbolsManager = new DefineSymbolsManager(new BuildTargetGroup[]
        {
            BuildTargetGroup.iOS,
            BuildTargetGroup.Android
        });
    }

    private void UpdateDefineSymbolsFromPlayerSettings()
    {
        defineSymbolsManager.UpdateFromBuildSettings();

        foreach (var item in defineSymbolProps)
        {
            if (item.Value == "UseVuforia")
            {
                var value = defineSymbolsManager.Has(item.Key) && PlayerSettings.GetPlatformVuforiaEnabled(BuildTargetGroup.Android) && PlayerSettings.GetPlatformVuforiaEnabled(BuildTargetGroup.iOS);
                UpdateDefineSymbolProp(item.Value, value);
            }
            else
            {
                UpdateDefineSymbolProp(item.Value, defineSymbolsManager.Has(item.Key));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateDefineSymbolProp(string propName, bool value)
    {
        var prop = serializedObject.FindProperty(propName);

        if (prop == null)
        {
            return;
        }

        prop.boolValue = value;
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        UpdateDefineSymbolsFromPlayerSettings();

        defineSymbolsManager.UpdateFromBuildSettings();


        EditorGUILayout.HelpBox("AR+GPS Location " + ARLocationConfig.Version, MessageType.None, true);
        EditorGUILayout.PropertyField(p_EarthRadiusInKM);
        EditorGUILayout.PropertyField(p_DistanceFunction);
        EditorGUILayout.PropertyField(p_UseNativeLocationModule);
        EditorGUILayout.PropertyField(p_UseVuforia);

        if (p_UseVuforia.boolValue)
        {
            EditorGUILayout.HelpBox("So that Vuforia works correctly, please enable the 'Track Device Pose' option in the Vuforia configuration, and set the tracking" +
                " mode to 'POSITIONAL'.", MessageType.Warning);
            EditorGUILayout.HelpBox(
                "Note that the regular sample scenes do not work with Vuforia. You can download a project with Vuforia samples at the releases page " +
                "in the GitHub repository. To get access to the repository send an email to daniel.mbfm@gmail.com with your github username.", MessageType.Warning);
        }

        if (GUILayout.Button("Compare Distance Functions (Check console output)"))
        {
            DistanceFunctionsTest();
        }

        if (GUILayout.Button("Open Documentation"))
        {
            Application.OpenURL("https://docs.unity-ar-gps-location.com");
        }
                
        var config = (ARLocationConfig) target;

        UpdateDefineSymbolPropConfig(config.UseVuforia, p_UseVuforia.boolValue, ARGPS_USE_VUFORIA);
        UpdateDefineSymbolPropConfig(config.UseNativeLocationModule, p_UseNativeLocationModule.boolValue, ARGPS_USE_NATIVE_LOCATION);

        UpdateVuforiaPlayerSettings(config.UseVuforia, p_UseVuforia.boolValue);

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateVuforiaPlayerSettings(bool oldValue, bool newValue)
    {
        if (newValue == oldValue)
        {
            return;
        }

        if (newValue)
        {
            PlayerSettings.SetPlatformVuforiaEnabled(BuildTargetGroup.Android, true);
            PlayerSettings.SetPlatformVuforiaEnabled(BuildTargetGroup.iOS, true);
        }
        else
        {
            PlayerSettings.SetPlatformVuforiaEnabled(BuildTargetGroup.Android, false);
            PlayerSettings.SetPlatformVuforiaEnabled(BuildTargetGroup.iOS, false);
        }
    }

    private void UpdateDefineSymbolPropConfig(bool oldValue, bool newValue, string symbol)
    {
        if (newValue != oldValue)
        {
            if (newValue)
            {
                defineSymbolsManager.Add(symbol);
            }
            else
            {
                defineSymbolsManager.Remove(symbol);
            }

            defineSymbolsManager.ApplyToBuildSettings();
        }
    }

    private void DistanceFunctionsTest()
    {
        var l1 = new Location(-23.591636, -46.661714);
        var l2 = new Location(-23.591661, -46.661695);

        var refValue = 3.380432468;
        var distHaversine = Location.HaversineDistance(l1, l2);
        var distSpherical = Location.PlaneSphericalDistance(l1, l2);
        var distFCC = Location.PlaneEllipsoidalFCCDistance(l1, l2);

        Debug.Log("Comparing short distance calculations...");
        Debug.Log("Location 1 = " + l1 + " Location 2 = " + l2);
        Debug.Log("Reference distance = " + refValue);
        Debug.Log("Haversine distance = " + distHaversine + " (Delta = " + System.Math.Abs(refValue - distHaversine) + ")");
        Debug.Log("Plane Spehrical distance = " + distSpherical + " (Delta = " + System.Math.Abs(refValue - distSpherical) + ")");
        Debug.Log("Plane Ellipsoidal FCC distance = " + distFCC + " (Delta = " + System.Math.Abs(refValue - distFCC) + ")");

        l2 = new Location(-23.593587, -46.660772);
        refValue = 236.504492294;
        distHaversine = Location.HaversineDistance(l1, l2);
        distSpherical = Location.PlaneSphericalDistance(l1, l2);
        distFCC = Location.PlaneEllipsoidalFCCDistance(l1, l2);

        Debug.Log("Comparing mid distance calculations...");
        Debug.Log("Location 1 = " + l1 + " Location 2 = " + l2);
        Debug.Log("Reference distance = " + refValue);
        Debug.Log("Haversine distance = " + distHaversine + " (Delta = " + System.Math.Abs(refValue - distHaversine) + ")");
        Debug.Log("Plane Spehrical distance = " + distSpherical + " (Delta = " + System.Math.Abs(refValue - distSpherical) + ")");
        Debug.Log("Plane Ellipsoidal FCC distance = " + distFCC + " (Delta = " + System.Math.Abs(refValue - distFCC) + ")");

        l2 = new Location(-23.606148, -46.653571);
        refValue = 1809.410855428;

        distHaversine = Location.HaversineDistance(l1, l2);
        distSpherical = Location.PlaneSphericalDistance(l1, l2);
        distFCC = Location.PlaneEllipsoidalFCCDistance(l1, l2);

        Debug.Log("Comparing long distance calculations...");
        Debug.Log("Location 1 = " + l1 + " Location 2 = " + l2);
        Debug.Log("Reference distance = " + refValue);
        Debug.Log("Haversine distance = " + distHaversine + " (Delta = " + System.Math.Abs(refValue - distHaversine) + ")");
        Debug.Log("Plane Spehrical distance = " + distSpherical + " (Delta = " + System.Math.Abs(refValue - distSpherical) + ")");
        Debug.Log("Plane Ellipsoidal FCC distance = " + distFCC + " (Delta = " + System.Math.Abs(refValue - distFCC) + ")");
    }
}
