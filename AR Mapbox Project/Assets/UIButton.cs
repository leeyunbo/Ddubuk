namespace Mapbox.Unity.Ar
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
            StartCoroutine(DelayLoadMessageRoutine(lat, lon));
                
        }

        IEnumerator DelayLoadMessageRoutine(double lat, double lon)
        {
            yield return new WaitForSeconds(1f);
            Message.Instance.LoadAllMessages(lat, lon);
        }
        
     
    }
}