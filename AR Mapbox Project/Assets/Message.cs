namespace Mapbox.Unity.Ar
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class Message : MonoBehaviour
    {
        private static Message _instance;
        public static Message Instance { get { return _instance; } }

        [HideInInspector]
        public double latitude;
        [HideInInspector]
        public double longitude;
        [HideInInspector]
        bool gpsInit = false;
        LocationInfo currentGPSPosition;
       
   
        public GameObject messagePrefabAR;
        Camera arCamera;
        List<GameObject> ObjectsList = new List<GameObject>();

        public void LoadAllMessages()
        {
            /*GameObject MessageBubble = Instantiate(messagePrefabAR); //오브젝트 생성
            Message message = MessageBubble.GetComponent<Message>(); // MessageBubble의 컴포넌트를 메시지 객체가 컨트롤 할 수 있게
            message.latitude = lat; // gps 지정
            message.longitude = lon; // 동일 
            ObjectsList.Add(MessageBubble); //오브젝트 리스트에 현재 오브젝트 저장
            ARMessageProvider.Instance.LoadARMessages(ObjectsList);*/  
        }
        /*IEnumerator GpsRoutine()
        {
            yield return new WaitForSeconds(15f);
            double lat = ARMessageProvider.Instance.deviceLocation.CurrentLocation.LatitudeLongitude.x;
            double lon = ARMessageProvider.Instance.deviceLocation.CurrentLocation.LatitudeLongitude.y;
            LoadAllMessages(lat, lon);
        }*/
        void Start()
        {
            arCamera = Camera.main;
            GameObject MessageBubble = Instantiate(messagePrefabAR);
            Message message = MessageBubble.GetComponent<Message>();

            //StartCoroutine(GpsRoutine());

            double lat = ARMessageProvider.Instance.deviceLocation.CurrentLocation.LatitudeLongitude.x;
            double lon = ARMessageProvider.Instance.deviceLocation.CurrentLocation.LatitudeLongitude.y;
  
            message.latitude = lat;
            message.longitude = lon;
            //LoadAllMessages(lat, lon);
           
            ARMessageProvider.Instance.LoadARMessages(MessageBubble);
        }

        void Update()
        {
            //make sure the bubble is always facing the camera
            transform.LookAt(Camera.main.transform);
        }
    }
}
