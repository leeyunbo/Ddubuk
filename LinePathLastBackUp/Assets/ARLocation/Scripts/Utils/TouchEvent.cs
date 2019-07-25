using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchEvent : MonoBehaviour
{
    public Text text;
    public Text UI;
    public Text RaycastID;
    public Text cube0ID;
    public Text cube1ID;
    public Text cube2ID;
    public Text cube3ID;

    private Ray ray;
    private RaycastHit hit;

    public GameObject cube0;
    public GameObject cube1;
    public GameObject cube2;
    public GameObject cube3;

    // Start is called before the first frame update

    void Start()
    {
        cube0ID.GetComponent<Text>().text = cube0.GetInstanceID().ToString();
        cube1ID.GetComponent<Text>().text = cube1.GetInstanceID().ToString();
        cube2ID.GetComponent<Text>().text = cube2.GetInstanceID().ToString();
        cube3ID.GetComponent<Text>().text = cube3.GetInstanceID().ToString();


    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                RaycastID.GetComponent<Text>().text = Mathf.Abs(hit.transform.gameObject.GetInstanceID()).ToString();
            }

             
        }

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
                    text.GetComponent<Text>().text = "터치누름";
                }
                else if(Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    text.GetComponent<Text>().text = "터치누르고이동";
                }
                else if(Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    text.GetComponent<Text>().text = "터치뗌";
                }
                if (hit.collider.gameObject != null)
                {
                    GameObject obj = hit.collider.gameObject;
                    print(obj.GetInstanceID());
                    UI.GetComponent<Text>().text = obj.GetInstanceID().ToString();
                }
            }
        }
        
    }
}
