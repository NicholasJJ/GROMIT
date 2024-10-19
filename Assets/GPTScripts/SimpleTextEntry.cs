using System;
using System.Collections;
using System.Collections.Generic;
using ChatGPTWrapper;
using UnityEngine;
using UnityEngine.Events;

public class SimpleTextEntry : MonoBehaviour
{
    [SerializeField] private String textToSend;
    public UnityStringEvent onClick;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("sending Message");
            // onClick.Invoke(textToSend);
            Debug.Log(TextTree.Instance.ToJson());
        }
       
        
    }
}
