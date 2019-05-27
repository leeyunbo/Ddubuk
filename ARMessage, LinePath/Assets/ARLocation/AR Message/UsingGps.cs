using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gps
{
    public class UsingGps : MonoBehaviour
    {
        private static UsingGps _instance;
        public static UsingGps Instance { get { return _instance; } }
        [HideInInspector]
        public double Dlatitude;
        public double Dlongitude;
        bool gpsStarted;
        float fiveSecondCounter = 0.0f;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            if (!Input.location.isEnabledByUser)
                yield break;

            Input.location.Start();

            int maxWait = 20;

            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait < 1)
            {
                print("Time out");
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                print("Unable to determine device location");
                yield break;
            }
            else
            {

                Dlatitude = Input.location.lastData.latitude;
                Dlongitude = Input.location.lastData.longitude;
                    
                
            }
        }

        void Update()
        {
            fiveSecondCounter += Time.deltaTime;
            if (fiveSecondCounter > 1.0)
            {
                UpdateGPS();
                fiveSecondCounter = 0.0f;
            }
        }

        void UpdateGPS()
        {
            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Input.location.Stop();
                Start();
            }
            else
            {
                Dlatitude = Input.location.lastData.latitude;
                Dlongitude = Input.location.lastData.longitude;
            }
        }


    
       


    }
}