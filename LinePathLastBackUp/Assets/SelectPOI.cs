using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectPOI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetString("SelectPOI").Equals("CAFE"))
        {
            PlayerPrefs.SetString("SelectPOI", "");
            SceneManager.LoadScene("POI_Cafe");
        }
    }
}
