using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.EventSystems;
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
        /// 
        public Sprite like;
        public Sprite dislike;

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

        public Dictionary<int, ARLocationManagerEntry> entries = new Dictionary<int, ARLocationManagerEntry>();

        GameObject PopupMessage;
        GameObject LoadingPopup;

        String UID;
        ARLocationManagerEntry ClickMessageInform;
        UserInformation UserInformation;
        private Ray ray;
        private RaycastHit hit;



        public List<Location> locations;

        ARLocationManager manager;
        UsingGps gps;
        Location location;

        public Text text;
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
            manager = ARLocationManager.Instance;
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
                        location.key = Convert.ToString(item.Child("key").Value);
                        location.altitude = Convert.ToDouble(item.Child("altitude").Value);
                        location.ignoreAltitude = Convert.ToBoolean(item.Child("ignoreAltitude").Value);
                        location.label = Convert.ToString(item.Child("label").Value);
                        location.latitude = Convert.ToDouble(item.Child("latitude").Value);
                        location.longitude = Convert.ToDouble(item.Child("longitude").Value);
                        location.uid = Convert.ToString(item.Child("uid").Value);
                        location.likecnt = Convert.ToInt32(item.Child("likecnt").Value);
                        foreach (var clickuid in item.Child("likelist").Children)
                        {
                            location.likelist.Add(Convert.ToString(clickuid.Value));
                        }
                        locations.Add(location);
                        AddLocation(location);
                    }
                }
            });
         
         

        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            if (manager == null)
            {
                Debug.LogError("[ARFoundation+GPSLocation][PlaceAtLocations]: ARLocatedObjectsManager Component not found.");

            }

            //HandleChildListener();           
            LoadingPopup.GetComponent<Canvas>().enabled = false;         
           
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity) && EventSystem.current.IsPointerOverGameObject() == false) 
                {
                    int id = Mathf.Abs(hit.collider.gameObject.GetInstanceID()) + 2;
                    if (id == (Mathf.Abs(manager.GetEntry(id).instance.GetInstanceID()) + 2))
                    {
                        getClickObjectInform(id);
                    }

                }
            }
        }


        public void writeNewMessage(double longitude, double latitude, string label) //DB�� �޽��� �߰� 
        {
            string key = databaseReference.Child("ARMessages").Push().Key;
            Location location = new Location();
            location.uid = this.UID; // �޽��� �ø���� UID
            location.key = key; // �޽��� DB key
            location.longitude = longitude; //longitude
            location.latitude = latitude; //latitude
            location.label = label; //�޽��� ����
            location.altitude = 1.0; // altitude
            location.likecnt = 0;
            string json = JsonUtility.ToJson(location);
            databaseReference.Child("ARMessages").Child(key).SetRawJsonValueAsync(json);
            //TestUpdateOnClickUID(location);
            AddLocation(location);
        }


        public void UpdateOnClickUID(ARLocationManagerEntry aRLocationManagerEntry)
        {
            Location location = aRLocationManagerEntry.location;
            string json = JsonUtility.ToJson(location);
            databaseReference.Child("ARMessages").Child(location.key).SetRawJsonValueAsync(json);
        }



        private void ARMessageLocation_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void AddLocation(Location location)
        {
            GameObject Prefab = thePrefab;           
            Prefab.GetComponentInChildren<Text>().text = location.label;
            Prefab.transform.GetChild(0).GetChild(0).Find("goodText").GetComponent<Text>().text = Convert.ToString(location.likecnt);
            if (isClickChecked(location) == true)
            {
                Prefab.transform.GetChild(0).GetChild(0).Find("heart").GetComponent<Text>().text = "<color=#ff0000>" + "��" + "</color>"; //��Ʈ �������� ���� 
            }
            //Prefab.transform.GetChild(0).Find("goodText").GetComponent<Text>().text = Convert.ToString(location.isClickUID.Count);


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

       /*
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
                location.likecnt = Convert.ToInt16(item.Child("likecnt").Value);
                
                
                
                locations.Add(location);
                AddLocation(location);             
                cnt++;
            }
        }

        public void HandleChildListener() //�����ͺ��̽��� ��ȭ(�߰�,����,����)�� ���� �����ʸ� ������ �޼ҵ�
        {
            databaseReference.Child("ARMessages").ValueChanged += HandleValueChanged;
        }*/

        public void GetUid() // Android ���� �Ѿ�� uid�� ��ü�� �������ִ� �޼ҵ�
        {
            this.UID = PlayerPrefs.GetString("Uid");
            UserInformation = new UserInformation(this.UID); //���� ���� ���� 
           
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

        public void getClickObjectInform(int instanceID) //������Ʈ�� ������.
        {
            this.ClickMessageInform = manager.GetEntry(instanceID); // ���� Ŭ���� ������Ʈ�� ���� ���� 
            ClickGoodButton();
            // ���� ���⼭ Ŭ���� ������Ʈ�� Location ��ü�� ������ ����� �� ����                
        }

        public void ClickGoodButton() //���ƿ� ��ư�� ����ڰ� ������.
        {                      
            if(isClickChecked(ClickMessageInform.location) == false) //���࿡ ��������� �ƴ϶�� 
            {
                ClickMessageInform.location.likelist.Add(this.UID); //�߰���, UID��
                ClickMessageInform.instance.transform.GetChild(0).GetChild(0).Find("heart").GetComponent<Text>().text = "<color=#ff0000>" + "��" + "</color>"; //������Ʈ
            }
            else //���� ���� ����̶��
            {
                ClickMessageInform.location.likelist.Remove(this.UID); // ������, UID��
                ClickMessageInform.instance.transform.GetChild(0).GetChild(0).Find("heart").GetComponent<Text>().text = "<color=#b0b0b0>" + "��" + "</color>"; //ȸ�� ��Ʈ 
            }
            ClickMessageInform.location.likecnt = ClickMessageInform.location.likelist.Count;
            ClickMessageInform.instance.transform.GetChild(0).GetChild(0).Find("goodText").GetComponent<Text>().text = Convert.ToString(ClickMessageInform.location.likecnt);
            UpdateOnClickUID(ClickMessageInform); //Ŭ���� ��� ���� DB ������Ʈ
     
        }



        public bool isClickChecked(Location location)
        {
            bool isClick = location.likelist.Contains(this.UID);
            return isClick;
        }






    }
}