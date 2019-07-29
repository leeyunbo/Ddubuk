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

public class UserInformation
{
    public string uid;
    public string userEmail;
    public string name;
    DatabaseReference databaseReference;
    FirebaseApp firebaseApp;

    public UserInformation(string uid)
    {
        firebaseApp = FirebaseDatabase.DefaultInstance.App;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://littletigers-44351.firebaseio.com");
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        databaseReference.Child("users").Child(uid).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Database Error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                this.uid = Convert.ToString(snapshot.Child("uid").Value);
                this.userEmail = Convert.ToString(snapshot.Child("email").Value);
                this.name = Convert.ToString(snapshot.Child("name").Value);
            }


        });
    }
}
