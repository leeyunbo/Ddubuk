using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progressbar : MonoBehaviour
{
    public Slider progressbar;
    public float maxvalue;

    // Start is called before the first frame update
    void Start()
    {
        progressbar.maxValue = maxvalue;
       

    }

    // Update is called once per frame
    void Update()
    {
        progressbar.value += Time.deltaTime * 5;
        
    }
}
