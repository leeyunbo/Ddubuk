using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using Gps;
using System.Threading;
namespace Gps
{

    /// <summary>
    /// Apply to a GameObject to place it at a specified geographic location.
    /// </summary>
    public class ARMessageLocation : MonoBehaviour
    {
        /// <summary>
        /// The location to place the GameObject at.
        /// </summary>


        /// <summary>
        /// If true, the altitude will be computed as relative to the device level.
        /// </summary>
        [Tooltip("If true, the altitude will be computed as relative to the device level.")]
        public bool isHeightRelative = true;

        /// <summary>
        /// If true, will display a UI panel with debug information above the object.
        /// </summary>
        [Tooltip("If true, will display a UI panel with debug information above the object.")]
        public bool showDebugInfoPanel = true;

        /// <summary>
        /// The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled.
        /// </summary>
        [Tooltip("The smoothing factor for movement due to GPS location adjustments; if set to zero it is disabled."), Range(0, 500)]
        public float movementSmoothingFactor = 120f;

        GameObject PopupMessage;
        GameObject LoadingPopup;

        public String UID;

        

        public List<Location> locations;

        ARLocationManager manager;
        UsingGps gps;
        Location location;

        public InputField Mlabel; // InputField
        public GameObject thePrefab; // ������Ʈ ��� 
      



        DatabaseReference databaseReference;
        FirebaseApp firebaseApp;

        // Use this for initialization
        public void Awake()
        {
            GetUid(); // ���� ������ Uid�� ������ �� ���� 
            PopupMessage = GameObject.Find("PopupMessage");
            LoadingPopup = GameObject.Find("LoadingPopup");
            PopupMessage.SetActive(false);
            gps = GameObject.Find("GpsMachine").GetComponent<UsingGps>();

            firebaseApp = FirebaseDatabase.DefaultInstance.App;
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://littletigers-44351.firebaseio.com");
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            FirebaseDatabase.DefaultInstance.GetReference("ARMessages").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Database Error");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (var item in snapshot.Children)
                    {
                        Location location = new Location();
                        location.altitude = Convert.ToDouble(item.Child("altitude").Value);
                        location.ignoreAltitude = Convert.ToBoolean(item.Child("ignoreAltitude").Value);
                        location.label = Convert.ToString(item.Child("label").Value);
                        location.latitude = Convert.ToDouble(item.Child("latitude").Value);
                        location.longitude = Convert.ToDouble(item.Child("longitude").Value);
                        location.UID = Convert.ToString(item.Child("Uid").Value);
                        locations.Add(location);
                        AddLocation(location);
                    }
                }
            });
         

        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            manager = ARLocationManager.Instance;

            if (manager == null)
            {
                Debug.LogError("[ARFoundation+GPSLocation][PlaceAtLocations]: ARLocatedObjectsManager Component not found.");

            }

            HandleChildListener();
            LoadingPopup.GetComponent<Canvas>().enabled = false;

        }


        public void writeNewMessage(double longitude, double latitude, string label) //DB�� �޽��� �߰� 
        {
            string key = databaseReference.Child("ARMessages").Push().Key;
            Location location = new Location();
            location.longitude = longitude;
            location.latitude = latitude;
            location.label = label;
            location.altitude = 1.0;
            location.UID = this.UID;
            string json = JsonUtility.ToJson(location);
            databaseReference.Child("ARMessages").Child(key).SetRawJsonValueAsync(json);
            //Relocation(location);
        }

        public void AddLocation(Location location)
        {
            GameObject Prefab = thePrefab;
            Prefab.GetComponentInChildren<Text>().text = location.label;

            manager.Add(new ARLocationManagerEntry
            {
                instance = Prefab,
                location = location,
                options = new ARLocationObjectOptions
                {
                    isHeightRelative = isHeightRelative,
                    showDebugInfoPanel = showDebugInfoPanel,
                    movementSmoothingFactor = movementSmoothingFactor,
                    createInstance = true,
                }
            });
        }

        void HandleValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            int cnt = 0;
            foreach (var item in args.Snapshot.Children)
            {
                Location location = new Location();
                location.altitude = Convert.ToDouble(item.Child("altitude").Value);
                location.ignoreAltitude = Convert.ToBoolean(item.Child("ignoreAltitude").Value);
                location.label = Convert.ToString(item.Child("label").Value);
                location.latitude = Convert.ToDouble(item.Child("latitude").Value);
                location.longitude = Convert.ToDouble(item.Child("longitude").Value);
                if (cnt == locations.Count)
                {
                    locations.Add(location);
                    AddLocation(location);
                }
                cnt++;
            }
        }

        public void HandleChildListener() //�����ͺ��̽��� ��ȭ(�߰�,����,����)�� ���� �����ʸ� ������ �޼ҵ�
        {
            databaseReference.Child("ARMessages").ValueChanged += HandleValueChanged;
        }

        public void GetUid() // Android ���� �Ѿ�� uid�� ��ü�� �������ִ� �޼ҵ�
        {
            this.UID = PlayerPrefs.GetString("Uid");
        }


        public void ClickButton() //�޽��� ���� ��ư Ŭ��
        {
            if (String.IsNullOrWhiteSpace(Mlabel.text) || Mlabel.text.Length > 60)
            {
                if (Mlabel.text.Length > 60)
                {
                    PopupMessage.transform.GetChild(1).GetComponent<Text>().text = "60�� ���Ϸ� �Է����ּ���.";
                }
                else
                {
                    PopupMessage.transform.GetChild(1).GetComponent<Text>().text = "�޽��� ������ �Է����ּ���.";
                }
                PopupMessage.SetActive(true);
                return;
            }
            writeNewMessage(gps.Dlongitude, gps.Dlatitude, Mlabel.text);
            Mlabel.text = "";
        }

        public void ClickPopupMessageButton() //�˾� �޽��� ���� �� ��ư Ŭ�� 
        {
            PopupMessage.SetActive(false);
            Mlabel.text = "";
        }

        public void getClickObjectInform(int instanceID)
        {
            ARLocationManagerEntry arLocationManagerEntry = manager.GetEntry(instanceID); //���⿡ instanceid�� �־����
            // ���� ���⼭ Ŭ���� ������Ʈ�� Location ��ü�� ������ ����� �� ���� 
        }






    }
}