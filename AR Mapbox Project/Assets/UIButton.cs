/*namespace Mapbox.Unity.Ar
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UIButton : MonoBehaviour
    {

        
            public void SubmitButtondown()
            {
                
                    double lat = ARMessageProvider.Instance.deviceLocation.CurrentLocation.LatitudeLongitude.x;
                    double lon = ARMessageProvider.Instance.deviceLocation.CurrentLocation.LatitudeLongitude.y;
                    Debug.Log("lat :" + lat + " " +"lon : " + lon);
                    Message.Instance.LoadAllMessages(lat, lon);
                
            }
        
     
    }
}*/