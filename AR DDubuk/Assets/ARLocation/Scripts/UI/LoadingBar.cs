using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour {

    [Range(0, 1)]
    public float fillPercentage = 0.4f;

    public Color startColor = Color.green;
    public Color middleColor = Color.yellow;
    public Color endColor = Color.red;
    public Color textColor = Color.blue;
    public bool usePercentageText = false;
    public string text = "100";

    private GameObject fillBar;
    private Text Text;

    // Use this for initialization
    void Start () {
        fillBar = transform.Find("Bar").gameObject;
        Text = transform.Find("Text").gameObject.GetComponent<Text>();
        Text.color = textColor;
        Text.fontStyle = FontStyle.Bold;
	}
	
	// Update is called once per frame
	void Update () {
        var w = GetComponent<RectTransform>().rect.width;

        fillBar.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        fillBar.GetComponent<RectTransform>().offsetMax = new Vector2((fillPercentage - 1) * w, 0);

        if (fillPercentage < 0.5)
        {
            fillBar.GetComponent<Image>().color = Color.Lerp(startColor, middleColor, fillPercentage * 2);
        } else
        {
            fillBar.GetComponent<Image>().color = Color.Lerp(middleColor, endColor, (fillPercentage - 0.5f) * 2);
        }

        if (usePercentageText)
        {
            Text.text = ((int) (fillPercentage * 100.0f)) + "%";
        }
        else
        {
            Text.text = text;
        }
    }
}
