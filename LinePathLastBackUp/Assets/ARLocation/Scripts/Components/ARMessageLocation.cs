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

        GameObject PopupMessage; // 오류 메시지 오브젝트 
   

        public List<Location> locations; // 메시지의 정보를 담아줄 리스트 

        ARLocationManager manager;
        UsingGps gps; // gps 객체
        Location location; // 메시지 정보 클래스 객체 

        public InputField Mlabel; // InputField
        public GameObject thePrefab; // 메시지 오브젝트 
    


        DatabaseReference databaseReference;
        FirebaseApp firebaseApp;  // 파이어베이스 객체 

        // Use this for initialization
        public void Awake()
        {
            PopupMessage = GameObject.Find("PopupMessage");
            PopupMessage.SetActive(false);
            gps = GameObject.Find("GpsMachine").GetComponent<UsingGps>(); // gps 센서 작동 시작 
            
            firebaseApp = FirebaseDatabase.DefaultInstance.App;
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://littletigers-44351.firebaseio.com");
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference; // 파이어베이스 접근

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
                        locations.Add(location);
                    }
                }
            });  //파이어베이스에 존재하는 메시지 관련 데이터 읽어오기
            
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            manager = ARLocationManager.Instance;

            if (manager == null)
            {
                Debug.LogError("[ARFoundation+GPSLocation][PlaceAtLocations]: ARLocatedObjectsManager Component not found.");
            
            }
        

            locations.ForEach(AddLocation); //메시지 정보들을 이용해, 해당 위치에 메시지를 띄어줌
            HandleChildListener(); //다른 사용자들 데이터베이스 접근 감지
        }

        public void update()
        {
            
        }

        public void ClickButton() //원하는 내용을 담은 메시지를 현재 자신의 위치에 띄우기
        {
            if(String.IsNullOrWhiteSpace(Mlabel.text) || Mlabel.text.Length > 150)       
            {
                if(Mlabel.text.Length > 150) //150자 이하로만 입력 가능
                {
                    PopupMessage.transform.GetChild(1).GetComponent<Text>().text = "150자 이하로 입력해주세요.";
                }
                else //만약 메시지 내용이 없으면 return 
                {
                    PopupMessage.transform.GetChild(1).GetComponent<Text>().text = "메시지 내용을 입력해주세요.";
                }
                PopupMessage.SetActive(true);
                return;
            }
            writeNewMessage( gps.Dlongitude, gps.Dlatitude, Mlabel.text);
            Mlabel.text = "";            
        }

        public void ClickPopupMessageButton() // 팝업 메시지 버튼 누르면 팝업 메시지 오브젝트 사라지게
        {
            PopupMessage.SetActive(false);
        }


        public void writeNewMessage(double longitude, double latitude, string label) //DB에 메시지 추가 
        {
            string key = databaseReference.Child("ARMessages").Push().Key;
            Location location = new Location();
            location.longitude = longitude;
            location.latitude = latitude;
            location.label = label;
            location.altitude = 1.0;
            string json = JsonUtility.ToJson(location);
            databaseReference.Child("ARMessages").Child(key).SetRawJsonValueAsync(json);
            //Relocation(location);
        }

        public void Relocation(Location location) //DB에 정보가 추가되면 다시 재위치
        {
            AddLocation(location);
        }

        public void AddLocation(Location location) //메시지 위치 시키기
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

        void HandleValueChanged(object sender, ValueChangedEventArgs args) //다른 사용자들이 데이터베이스 접근하는 것 감지
        {
            if(args.DatabaseError != null)
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
                    Relocation(location);
                }
                cnt++;
            }
            print("value changed");
            
        }

        public void HandleChildListener()
        {
            databaseReference.Child("ARMessages").ValueChanged += HandleValueChanged;
        }




    }
}