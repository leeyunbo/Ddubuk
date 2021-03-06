﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gps {
    public class TouchEvent : MonoBehaviour
    {
        private Ray ray;
        private RaycastHit hit;
        private GameObject gameObject;

        void Awake()
        {
            gameObject = GameObject.Find("ARSession Origin");
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.gameObject != null)
                {
                    gameObject.transform.GetChild(1).SendMessage("getClickObjectInform", Mathf.Abs(hit.collider.gameObject.GetInstanceID())); 
                    
                }

            }
        }
    }
}
