namespace Mapbox.Unity.Ar
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Mapbox.Unity.Utilities;
    using Mapbox.Utils;
    using Mapbox.Unity.Map;
    public class ARMessageProvider : MonoBehaviour
    {
        private static ARMessageProvider _instance;
        public static ARMessageProvider Instance { get { return _instance; } }

        [SerializeField]
        private AbstractMap _map;

        [HideInInspector]
        GameObject currentMessages;

        [HideInInspector]
        public bool deviceAuthenticated = false;
        private bool gotInitialAlignment = false;

        public Mapbox.Unity.Location.DeviceLocationProvider deviceLocation;
        // Start is called before the first frame update


        void Awake()
        {
            _instance = this;
        }


        public void LoadARMessages(GameObject messageObject)
        {
           
            StartCoroutine(LoadARMessagesRoutine(messageObject));
        
        }

        IEnumerator LoadARMessagesRoutine(GameObject messageObject)
        { //이미 존재하는 게임오브젝트에 대한 정보를 가져온다.
            
            yield return new WaitForSeconds(2f);

            /*foreach(GameObject messageObject in messageObjectList)
            {
                Message thisMessage = messageObject.GetComponent<Message>();
                Vector3 _targetPosition = Conversions.GeoToWorldPosition(thisMessage.latitude, thisMessage.longitude, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz();
                messageObject.transform.position = _targetPosition;
                currentMessages.Add(messageObject);         
            }*/
            
                Message thisMessage = messageObject.GetComponent<Message>();               
                Vector3 _targetPosition = Conversions.GeoToWorldPosition(thisMessage.latitude, thisMessage.longitude, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz();
       
                
 
                messageObject.transform.position = _targetPosition;
                
            //add to list so we can update positions later
                currentMessages = messageObject;
             
        }

        /*public void UpdateARMessageLocations(Vector2d currentLocation)
        {
            if (currentMessages.Count > 0)
            {
                foreach (GameObject messageObject in currentMessages)
                {
                    Message message = messageObject.GetComponent<Message>();
                    Vector3 _targetPosition = Conversions.GeoToWorldPosition(message.latitude, message.longitude, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz();
                    messageObject.transform.position = _targetPosition;
                }
            }
       
        }
*/
    }
}
