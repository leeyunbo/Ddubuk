using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutTextMesh : MonoBehaviour {

    public float duration = 2.0f;

	// Use this for initialization
	void Start () {
        StartCoroutine("FadeOut");
	}

    IEnumerator FadeOut()
    {
        var textMesh = GetComponent<TextMesh>();
        var t = duration;
        var initialColor = textMesh.color;
        while (textMesh.color.a > 0.001f)
        {
            var target = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0);
            textMesh.color = Color.Lerp(initialColor, target, 1 - t/duration);
            t -= Time.deltaTime;
            yield return null;
        }
    }
}
