using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMelon : MonoBehaviour
{

    public float rotationSpeed = 20.0f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}