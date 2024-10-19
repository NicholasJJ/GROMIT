using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitzCamera : MonoBehaviour
{
    private Quaternion cachedRotation;
    private Vector3 cachedOffset;

    // Start is called before the first frame update
    void Start()
    {
        cachedRotation = transform.rotation;
        cachedOffset = transform.position - transform.parent.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = cachedRotation;
        transform.position = transform.parent.position + cachedOffset;
    }
}
