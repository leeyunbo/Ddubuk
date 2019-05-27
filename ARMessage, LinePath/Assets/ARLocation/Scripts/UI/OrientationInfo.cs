using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrientationInfo : MonoBehaviour {
    private ARLocationOrientation orientation;
    private GameObject redArrow;
    private GameObject blueArrow;
    private GameObject trueNorthLabel;
    private GameObject magneticNorthLabel;
    private GameObject headingAccuracyLabel;
    private GameObject compassImage;
    private ARLocationProvider locationProvider;

    // Use this for initialization
    void Start () {
        orientation = FindObjectOfType<ARLocationOrientation>();
        locationProvider = ARLocationProvider.Instance;

        redArrow = GameObject.Find(gameObject.name + "/Panel/CompassImage/RedArrow");
        blueArrow = GameObject.Find(gameObject.name + "/Panel/CompassImage/BlueArrow");
        trueNorthLabel = GameObject.Find(gameObject.name + "/Panel/TrueNorthLabel");
        magneticNorthLabel = GameObject.Find(gameObject.name + "/Panel/MagneticNorthLabel");
        headingAccuracyLabel = GameObject.Find(gameObject.name + "Panel/HeadingAccuracyLabel");
        compassImage = GameObject.Find(gameObject.name + "Panel/CompassImage");
        
    }
	
	// Update is called once per frame
	void Update () {
        var mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        if (mainCamera == null)
        {
            return;
        }

        var orientationAngle = orientation.transform.localEulerAngles.y;
        var cameraAngle = mainCamera.transform.localEulerAngles.y;
        var currentCompassAngle = cameraAngle - orientationAngle;

        var currentHeading = locationProvider.currentHeading.heading;
        var currentMagneticHeading = locationProvider.currentHeading.magneticHeading;
        var currentAccuracy = locationProvider.provider.currentHeading.accuracy;

        trueNorthLabel.GetComponent<Text>().text = "TRUE NORTH: " + currentHeading;
        magneticNorthLabel.GetComponent<Text>().text = "MAGNETIC NORTH: " + currentMagneticHeading;
        headingAccuracyLabel.GetComponent<Text>().text = "ACCURACY: " + currentAccuracy;

        redArrow.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, (float) currentMagneticHeading);
        compassImage.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, (float)currentHeading);
    }
}
