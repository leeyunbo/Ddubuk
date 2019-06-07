using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraPosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        var cameraPos = Camera.main.transform.position;

        transform.position = new Vector3(
            cameraPos.x,
            transform.position.y,
            cameraPos.z
        );
	}
}
