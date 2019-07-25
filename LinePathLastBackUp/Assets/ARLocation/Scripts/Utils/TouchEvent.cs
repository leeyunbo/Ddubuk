using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gps {
    public class TouchEvent : MonoBehaviour
    {
        private Ray ray;
        private RaycastHit hit;
        public Text text;




        // Start is called before the first frame update
        


        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    text.GetComponent<Text>().text = hit.collider.gameObject.GetInstanceID().ToString();
                    GameObject.Find("ARLocationRoot").SendMessage("getClickObjectInform", hit.transform.gameObject.GetInstanceID());
                }


            }



        }
    }
}
