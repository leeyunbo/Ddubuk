using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Vector2 pos = Input.GetTouch(0).position;
            Vector3 theTouch = new Vector3(pos.x, pos.y, 0.0f);

            Ray ray = Camera.main.ScreenPointToRay(theTouch);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    print("터치 누름");
                }
                else if(Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    print("터치 누르고 이동");
                }
                else if(Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    print("터치 뗌");
                }
                GameObject obj = hit.collider.gameObject;
                print(obj.GetInstanceID());
            }
        }
        
    }
}
