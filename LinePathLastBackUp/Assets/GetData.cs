using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetData : MonoBehaviour
{
    // 메세지, POI, 길찾기 타입 받는 함수
    public void selectScene(string scene)
    {
        PlayerPrefs.SetString("Scene", scene);
    }

    // 위치경로 받는 함수
    public void RecvLocation(string loc)
    {
        PlayerPrefs.SetString("LocationPath", loc);
    }

    // POI 정보 받는 함수들
    public void CAFE(string infor)
    {
        PlayerPrefs.SetString("CAFE", infor);
    }

    public void BUSSTOP(string infor)
    {
        PlayerPrefs.SetString("BUSSTOP", infor);
    }

    public void CONVENIENCE(string infor)
    {
        PlayerPrefs.SetString("CONVENIENCE", infor);
    }

    public void RESTAURANT(string infor)
    {
        PlayerPrefs.SetString("RESTAURANT", infor);
    }

    public void BANK(string infor)
    {
        PlayerPrefs.SetString("BANK", infor);
    }

    public void ACCOMMODATION(string infor)
    {
        PlayerPrefs.SetString("ACCOMMODATION", infor);
    }

    public void HOSPITAL(string infor)
    {
        PlayerPrefs.SetString("HOSPITAL", infor);
    }

    private void Awake()
    {
        PlayerPrefs.SetString("Scene", " ");
    }

    // Start is called before the first frame update
    void Start()
    {     
    }

    // Update is called once per frame
    void Update()
    {
        // Scene이 Nav 일때 씬 이동
        if (PlayerPrefs.GetString("Scene").Equals("NAV"))
        {
            SceneManager.LoadScene("Location Path");
        }
        // Scene이 POI 일때 씬 이동
        else if (PlayerPrefs.GetString("Scene").Equals("POI"))
        {
            SceneManager.LoadScene("POI_Main");
        }
        // Scene이 Msg 일때 씬 이동
        else if (PlayerPrefs.GetString("Scene").Equals("MSG"))
        {
            SceneManager.LoadScene("AR Message");
        }
        else
        { }

    }
}
