using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageService : MonoBehaviour
{
    // Start is called before the first frame update
    private static MessageService _instance;
    public static MessageService Instance { get { return _instance; } }

    public Transform mapRootTransform;

    public GameObject messagePrefabAR;

    private void Awake()
    {
        _instance = this;
    }
    
    
}
