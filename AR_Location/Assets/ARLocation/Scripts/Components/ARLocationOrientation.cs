using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component should be placed on the "ARLocationRoot" GameObject (which should be a child of the
/// "AR Session Origin") for correctly aligning the coordinate system to the north/east geographical lines.
/// </summary>
public class ARLocationOrientation : MonoBehaviour
{
    /// <summary>
    /// Only update after measuring the heading N times, and take the average.
    /// </summary>
    [Tooltip("Only update after measuring the heading N times, and take the average."), Range(1, 500)]
    public int averageCount = 250;

    /// <summary>
    /// If true, only measure during the session's start-up. If false, update the heading every time we calculate an average value.
    /// Setting to false will give more accurate results but may cause some jittering in the scene object's.
    /// </summary>
    [Tooltip("If true, only measure during the session's start-up. If false, update the heading every time we calculate an average value." +
             " Setting to false will give more accurate results but may cause some jittering in the scene object's.")]
    public bool measureOnlyAtStartup = false;

    /// <summary>
    /// If set to true, use raw heading values until measuring the first average.
    /// </summary>
    [Tooltip("If set to true, use raw heading values until measuring the first average.")]
    public bool useRawUntilFirstAverage = true;

    /// <summary>
    /// If set to true, disable movement smoothing until we get a first averaged value.
    /// </summary>
    [Tooltip("If set to true, disable movement smoothing until we get a first averaged value.")]
    public bool noSmoothingBeforeFirstAverage = true;

    /// <summary>
    /// The smoothing factor. Zero means disabled. Values around 100 seem to give good results.
    /// </summary>
    [Tooltip("The smoothing factor. Zero means disabled. Values around 100 seem to give good results."), Range(0, 500)]
    public float smoothing = 50.0f;

    /// <summary>
    /// A custom offset to the device-calculated true north direction.
    /// </summary>
    [Tooltip("A custom offset to the device-calculated true north direction.")]
    public float TrueNorthCalibrationOffset = 0.0f;

    ARLocationProvider locationProvider;

    int measurementCount = 0;
    List<float> values = new List<float>();
    bool firstRun = true;
    bool firstCall = true;
    float target = 0.0f;
    private ARLocationDebugCanvas debugCanvas;

    /// <summary>
    /// Restarts the orientation tracking.
    /// </summary>
    public void Restart()
    {
        measurementCount = 0;
        firstRun = true;
        firstCall = true;
        target = Camera.main.transform.rotation.eulerAngles.y;
    }

    // Use this for initialization
    void Start()
    {
        // Look for the LocationProvider
        locationProvider = ARLocationProvider.Instance;

        target = Camera.main.transform.rotation.eulerAngles.y;

        // Register compass update delegate
        locationProvider.OnCompassUpdated((newHeading, lastHeading) =>
        {

            if (!newHeading.isMagneticHeadingAvailable)
            {
                return;
            }

            var isRawTime = locationProvider.provider.isRawTime();

            var trueHeading = newHeading.heading + TrueNorthCalibrationOffset;

            // If measureOnlyAtStartup is true, and we already averaged once, do nothing
            if (measureOnlyAtStartup && !firstRun)
            {
                return;
            }

            // Get current heading value
            float currentCameraHeading = Camera.main.transform.rotation.eulerAngles.y;
            float value = Utils.GetNormalizedDegrees(currentCameraHeading - ((float)trueHeading));

            if (isRawTime)
            {
                transform.localRotation = Quaternion.AngleAxis(value, Vector3.up);
                target = value;
                return;
            }

            // If N = 1, only apply the raw value
            if (averageCount <= 1)
            {
                // transform.localRotation = Quaternion.AngleAxis(value, Vector3.up);
                target = value;

                if (firstRun)
                {
                    firstRun = false;
                }

                return;
            }

            // Add current value to the values list, for averaging
            values.Add(value);

            measurementCount++;

            // If it's the very first call, apply the value
            if (firstCall && Mathf.Abs(value) >= 0.00001f)
            {
                // transform.localRotation = Quaternion.AngleAxis(value, Vector3.up);
                target = value;
                firstCall = false;
            }

            if (useRawUntilFirstAverage && firstRun)
            {
                if (noSmoothingBeforeFirstAverage)
                {
                    transform.localRotation = Quaternion.AngleAxis(value, Vector3.up);
                }

                target = value;
            }

            // If we measured enough, calculate the average and apply it
            if (measurementCount >= averageCount)
            {
                var average = Utils.FloatListAverage(values);

                measurementCount = 0;
                values.Clear();

                target = average;

                if (firstRun)
                {
                    firstRun = false;
                }

            }
        });

        var debugCanvasObject = GameObject.Find("ARLocationDebugCanvas");

        if (debugCanvasObject != null)
        {
            debugCanvas = debugCanvasObject.GetComponent<ARLocationDebugCanvas>();
        }
    }

    private void Update()
    {
        if (locationProvider.provider.isRawTime())
        {
            return;
        }

        if (locationProvider.provider == null || !locationProvider.provider.isCompassEnabled)
        {
            return;
        }

        if (Mathf.Abs(transform.rotation.eulerAngles.y - target) <= 0.05f)
        {
            return;
        }

        var value = Mathf.Lerp(transform.rotation.eulerAngles.y, target, Mathf.Exp(-smoothing * Time.deltaTime));
        transform.localRotation = Quaternion.AngleAxis(value, Vector3.up);
    }
}
