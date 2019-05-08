using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugInfoOverlay : MonoBehaviour {

    public bool show = false;
    public bool showObjectInfo = false;

    private GameObject canvas;
    private GameObject canvas2;
    private GameObject btn1;
    private GameObject btn2;

    // Use this for initialization
    void Start () {
        canvas = GameObject.Find(gameObject.name + "/Canvas");
        canvas2 = GameObject.Find(gameObject.name + "/ObjectInfoCanvas");
        btn1 = GameObject.Find(gameObject.name + "/ButtonCanvas/ToggleInfoButton");
        btn2 = GameObject.Find(gameObject.name + "/ButtonCanvas/ToggleInfoButton2");

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        canvas.SetActive(show);
        canvas2.SetActive(showObjectInfo);

        if (!show)
        {
            btn1.GetComponentInChildren<Text>().text = "Show Info Overlay";
        }
        else
        {
            btn1.GetComponentInChildren<Text>().text = "Hide Info Overlay";
        }

        if (!showObjectInfo)
        {
            btn2.GetComponentInChildren<Text>().text = "Show Object Info";
        }
        else
        {
            btn2.GetComponentInChildren<Text>().text = "Hide Object Info";
        }
    }

    public void Toggle()
    {
        show = !show;
        UpdateInfo();
    }

    public void ToggleObjectInfo()
    {
        showObjectInfo = !showObjectInfo;
        UpdateInfo();
    }
}
