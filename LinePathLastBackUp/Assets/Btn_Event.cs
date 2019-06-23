using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Btn_Event : MonoBehaviour
{
    // Start is called before the first frame update    

    ARLocationManager manager;

    public List<GameObject> Btns;

    public GameObject menuBtn;

    public void BtnMenu()
    {               
        if (PlayerPrefs.GetString("hide").Equals("false"))
        {
            menuBtn.GetComponent<Text>().text = "메뉴▼";

            for (var i = 0; i < 7; i++)
            {
                Btns[i].SetActive(false);
                PlayerPrefs.SetString("hide", "true");
            }
        }
        else
        {
            menuBtn.GetComponent<Text>().text = "접기▲";
            
            for (var i = 0; i < 7; i++)
            {
                Btns[i].SetActive(true);
                PlayerPrefs.SetString("hide", "false");
            }
        }
    }

    public void BtnCafe()
    {
        SceneManager.LoadScene("POI_Cafe");
    }

    public void BtnBusstop()
    {
        SceneManager.LoadScene("POI_Busstop");
    }

    public void BtnRestaurant()
    {
        SceneManager.LoadScene("POI_Restaurant");
    }

    public void BtnConvenience()
    {
        SceneManager.LoadScene("POI_Convenience");
    }

    public void BtnBank()
    {
        SceneManager.LoadScene("POI_Bank");
    }

    public void BtnHospital()
    {
        SceneManager.LoadScene("POI_Hospital");
    }

    public void BtnAccommodation()
    {
        SceneManager.LoadScene("POI_Accommodation");
    }

    void Start()
    {
        if (PlayerPrefs.GetString("hide").Equals("false"))
        {
            menuBtn.GetComponent<Text>().text = "접기▲";
        }
        else
        {
            for (int i = 0; i < 7; i++)
            {
                Btns[i].SetActive(false);
                PlayerPrefs.SetString("hide", "true");
            }
        }
    }
    
    void Update()
    {   
    }
}
