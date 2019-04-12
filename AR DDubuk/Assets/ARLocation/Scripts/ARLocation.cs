using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This static class loads the global configuration for the AR + GPS Location
/// plugin. 
/// 
/// Any other global functionality of the plugin should be placed here as 
/// well.
/// </summary>
static class ARLocation {
    public static ARLocationConfig config;

    static ARLocation()
    {
        config = Resources.Load<ARLocationConfig>("ARLocationConfig");

        if (config == null)
        {
            Debug.LogWarning("Resources/ARLocationConfig.asset not found; creating new configuration from defaults.");
            config = new ARLocationConfig();
        }
    }
}
