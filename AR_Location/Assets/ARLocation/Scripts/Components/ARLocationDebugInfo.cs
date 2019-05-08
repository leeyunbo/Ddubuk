using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Positions UI Panels with debug information on top of objects managed by 
/// a ARLocatedObjectsManager.
/// </summary>
public class ARLocationDebugInfo : MonoBehaviour
{
    /// <summary>
    /// A screen space overlay UI Canvas that will hold the panels.
    /// </summary>
    [Tooltip("A screen space overlay UI Canvas that will hold the panels.")]
    public Canvas canvas;

    /// <summary>
    /// A UI prefab containing a text object.
    /// </summary>
    [Tooltip("A UI prefab containing a text object.")]
    public GameObject debugInfoPrefab;

    /// <summary>
    /// Store the info panel instances in a dictionary indexed by the instance IDs of the
    /// object's transform component.
    /// </summary>
    Dictionary<int, GameObject> debugPanelIntances = new Dictionary<int, GameObject>();

    ARLocationManager manager;
    ARLocationProvider locationProvider;

    private void Awake()
    {
        // Make sure our delegate is registered beforehand
        manager = ARLocationManager.Instance;
        manager.OnObjectAdded(HandleOnObjectAddedDelegate);

        if (canvas == null)
        {
            var newCanvas = GameObject.Find("ARLocationInfo/ObjectInfoCanvas");

            if (newCanvas != null)
            {
                canvas = newCanvas.GetComponent<Canvas>();
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        locationProvider = ARLocationProvider.Instance;

        if (canvas == null)
        {
            return;
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    void HandleOnObjectAddedDelegate(ARLocationManagerEntry entry)
    {
        if (!entry.options.showDebugInfoPanel)
        {
            return;
        }

        var label = Instantiate(debugInfoPrefab, canvas.transform);
        label.GetComponentInChildren<Text>().text = entry.instance.name;

        debugPanelIntances.Add(entry.instance.transform.GetInstanceID(), label);
    }


    // Update is called once per frame
    void Update()
    {
        UpdateDebugInfoPanelPositions();
    }

    private void UpdateDebugInfoPanelPositions()
    {
        if (!locationProvider.isEnabled)
        {
            return;
        }

        // Update debug info label positions
        foreach (var item in debugPanelIntances)
        {
            updateDebugInfoPanelScreenPosition(item.Key, item.Value);
        }
    }

    private void updateDebugInfoPanelScreenPosition(int id, GameObject panel)
    {
        var entry = manager.GetEntry(id);
        var instance = entry.instance;
        var location = entry.location;

        var text = instance.name + "\n"
            + "LAT: " + location.latitude + "\n"
            + "LNG: " + location.longitude + "\n"
            + "ALT: " + location.altitude + "\n"
            + "POS: " + instance.transform.position
            + "DST: " + Location.HorizontalDistance(locationProvider.currentLocation.ToLocation(), location);

        ARLocationDebugInfo.UpdateDebugInfoPanelScreenPositionAndText(instance, panel, text);
    }

    static public void UpdateDebugInfoPanelScreenPositionAndText(GameObject target, GameObject panel, string text)
    {
        var screenPoint = GetDebugInfoPanelScreenPosition(target);

        panel.GetComponent<RectTransform>().position = screenPoint;

        if (screenPoint.z < 0)
        {
            panel.SetActive(false);
        }
        else if (!panel.activeSelf || !panel.activeInHierarchy)
        {
            panel.SetActive(true);
        }

        panel.GetComponentInChildren<Text>().text = text;
    }

    /// <summary>
    /// Returns the screen position just above a GameObject which has a MeshRenderer component (in itself or in one of its children).
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    static public Vector3 GetDebugInfoPanelScreenPosition(GameObject gameObject)
    {
        var meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();

        var extend = 0.0f;
        if (meshRenderer != null)
        {
            extend = meshRenderer.bounds.extents.y;
        }

        return Camera.main.WorldToScreenPoint(
            new Vector3(
                gameObject.transform.position.x,
                gameObject.transform.position.y  + extend,
                gameObject.transform.position.z
            )
        );
    }
}
