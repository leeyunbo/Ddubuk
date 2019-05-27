using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)

        {

            if (Input.GetKey(KeyCode.Escape))
            {
                // 백버튼 누르면 애플리케이션 종료
                Application.Quit();
#if !UNITY_EDITOR
                System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
            }
        }
    }
}
