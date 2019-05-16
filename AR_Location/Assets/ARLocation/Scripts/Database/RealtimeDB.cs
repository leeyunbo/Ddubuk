using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using UnityEngine.UI;

public class RealtimeDB : MonoBehaviour
{
    [Tooltip("The location to place the GameObject at.")]
    [SerializeField]
    public List<Location> locations;

    [SerializeField]
    public Location location;
    [SerializeField]
    ARLocationPlaceAtLocation ARLocation;
    [SerializeField]
    DatabaseReference databaseReference;
    [SerializeField]
    FirebaseApp firebaseApp;
    [SerializeField]
    public int cnt = 0; // 오브젝트 갯수 

    [SerializeField]
    public InputField m_label; // InputField

    [SerializeField]
    string label;

    public List<Location> StartDB()
    { 
        firebaseApp = FirebaseDatabase.DefaultInstance.App;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://artest-13270.firebaseio.com/");
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseApp.DefaultInstance.SetEditorP12FileName("artest-13270-04f10cf6189d.p12");
        FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("artest-13270@appspot.gserviceaccount.com");
        FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");

        FirebaseDatabase.DefaultInstance.GetReference("Messages").GetValueAsync().ContinueWith(task =>
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

                    print("altitude: " + location.altitude);
                    print("ignoreAltitude: " + location.ignoreAltitude);
                    print("label: " + location.label);
                    print("latitude: " + location.latitude);
                    print("longitude: " + location.longitude);
                    print("----------------------------------------------------------------------------");
                    locations.Add(location);
                    cnt++; // 오브젝트 갯수 

                }
            }
        });
        return locations;
    }

    public void ClickButton()
    {
        print("Click button!!");
        label = m_label.text;
        location = location.MClone();
        location.label = label;
        writeNewMessage(cnt, location);
        cnt++;
        
    }

    public void writeNewMessage(int mid, Location location) //DB에 메시지 추가 
    {
        string json = JsonUtility.ToJson(location);
        databaseReference.Child("Messages").Child(mid.ToString()).SetRawJsonValueAsync(json);
        ARLocation.Relocation(location);
    }

    // Update is called once per frame

}
