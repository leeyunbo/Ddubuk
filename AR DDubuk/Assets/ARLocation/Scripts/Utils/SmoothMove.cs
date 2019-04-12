using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMove : MonoBehaviour {

    [Tooltip("The smoothing factor."), Range(0, 500)]
    public float smoothing = 120f;

    private Vector3 target;

    public Vector3 Target
    {
        get { return target; }
        set {
            target = value;

            StopCoroutine("MoveTo");
            StartCoroutine("MoveTo", target);
        }
    }

    SmoothMove(float smoothing = 7f) {
        this.smoothing = smoothing;
    }

    IEnumerator MoveTo(Vector3 p_target) {
        while (Vector3.Distance(transform.position, p_target) > 0.05f) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, p_target, Mathf.Exp(-smoothing * Time.deltaTime));
            yield return null;
        }
    }
}
