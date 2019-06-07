using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Application_Event : MonoBehaviour
{
    bool bPaused = false;

    void OnApplicationPause(bool pause)
    {
        if (pause)

        {
            bPaused = true;
            // todo : 어플리케이션을 내리는 순간에 처리할 행동들 /
        }
        else
        {
            if (bPaused)
            {
                bPaused = false;
                //todo : 내려놓은 어플리케이션을 다시 올리는 순간에 처리할 행동들 
            }
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("hide", "true");
        // todo : 어플리케이션을 종료하는 순간에 처리할 행동들
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}


