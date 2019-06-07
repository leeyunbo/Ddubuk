using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This is a static class that makes sure that there always is a 
/// ARLocationConfig resource for the project.
/// </summary>
[InitializeOnLoad]
public class ARLocationEditorConfigManager {
    static ARLocationEditorConfigManager()
    {
        Debug.Log("[ARLocation]: Starting up!");

        if (AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            Debug.Log("[ARLocation]: Resource folder already exists!");
        }
        else
        {
            Debug.Log("[ARLocation]: Creating resource folder...");
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        var ss = AssetDatabase.FindAssets("ARLocationConfig", new string[] {"Assets/Resources"});

        if (ss.Length > 0)
        {
            Debug.Log("[ARLocation]: Config already exists!");
        }
        else
        {
            Debug.Log("[ARLocation]: Creating new configuration!");
            AssetDatabase.CopyAsset("Assets/ARLocation/ARLocationConfig.asset", "Assets/Resources/ARLocationConfig.asset");
        }

    }
}
