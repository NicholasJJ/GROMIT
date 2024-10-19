using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnotateLog : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) Debug.Log("Should be Yes");
        if (Input.GetKeyDown(KeyCode.N)) Debug.Log("Should be No");
    }
}
