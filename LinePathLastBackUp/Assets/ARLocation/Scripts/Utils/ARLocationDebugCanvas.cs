using System;
using UnityEngine;
using UnityEngine.UI;

public class ARLocationDebugCanvas : MonoBehaviour
{

    GameObject latValueText;
    GameObject lngValueText;
    GameObject headingValueText;
    GameObject altitudeValueText;
    GameObject debugText;
    ARLocationProvider locationProvider;

    float firstHeading;

    // Use this for initialization
    void Start()
    {
        latValueText = GameObject.Find("LatValue");
        lngValueText = GameObject.Find("LngValue");
        headingValueText = GameObject.Find("HeadingValue");
        altitudeValueText = GameObject.Find("AltValue");
        debugText = GameObject.Find("DebugText");


        locationProvider = ARLocationProvider.Instance;

        //locationProvider.onLocationUpdated((Location location, Location _, Vector3 __, float accuracy) =>
        //{
        //    setLat(location.latitude, accuracy);
        //    setLng(location.longitude);
        //    setAltitude(location.altitude);
        //});

        //locationProvider.onCompassUpdated(setHeading);
    }

    private object setHeading(object arg1)
    {
        throw new NotImplementedException();
    }

    void setLat(double val, float accuracy)
    {
        latValueText.GetComponent<Text>().text = val.ToString();
    }

    void setLng(double val)
    {
        lngValueText.GetComponent<Text>().text = val.ToString();
    }

    void setHeading(double val)
    {
        headingValueText.GetComponent<Text>().text = val.ToString();
    }

    void setAltitude(double val) {
        altitudeValueText.GetComponent<Text>().text = val.ToString();
    }

    public void SetDebugText(string val)
    {
        debugText.GetComponent<Text>().text = val;
    }

    // Update is called once per frame
    void Update()
    {
        // GameObject.Find("DebugText").GetComponent<Text>().text = "cameraPos: " + Camera.main.transform.position;
    }
}
