namespace Mapbox.Unity.Ar
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class Message : MonoBehaviour
    {
        [HideInInspector]
        public double latitude;
        [HideInInspector]
        public double longitude;
        [HideInInspector]
        public string text;
        [HideInInspector]
        bool gpsInit = false;
        LocationInfo currentGPSPosition;
        int gps_connect = 0;
        double detailed_num = 1.0;
        public Text text_latitude;
        public Text text_longitude;
        public Transform mapRootTransform;
        public GameObject messagePrefabAR;

        public TextMesh messageText;

        public void SetText(string text)
        {
            //TODO: here we would need to size the text and 
            //mesage bubble according to the length of text.
            //right now this only replaces spaces with a new 
            //line character
            string newText = text.Replace(" ", "\n");
            messageText.text = newText;
        }
        void Start()
        {
            GameObject MessageBubble = Instantiate(messagePrefabAR, mapRootTransform);
            Message message = MessageBubble.GetComponent<Message>();
            //set the camera in the text's Canvas component, I needed to add this
            //so the layer of the text always shows up over the message sprite.
            
            messageText.GetComponent<Canvas>().worldCamera = Camera.main;
            double lat = ARMessageProvider.Instance.deviceLocation.CurrentLocation.LatitudeLongitude.x;
            double lon = ARMessageProvider.Instance.deviceLocation.CurrentLocation.LatitudeLongitude.y;
            message.latitude = lat;
            message.longitude = lon;
            ARMessageProvider.Instance.LoadARMessages(MessageBubble);
        }

        void Update()
        {
            //make sure the bubble is always facing the camera
            transform.LookAt(Camera.main.transform);
        }
    }
}
