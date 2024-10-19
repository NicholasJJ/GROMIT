using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicTransform : MonoBehaviour
{
    [SerializeField] GameObject mimicPrefab;
    [SerializeField] Transform mimicArea;
    Transform mimicObject;
    // Start is called before the first frame update
    void Start()
    {
        mimicObject = Instantiate(gameObject, mimicArea).transform;
    }

    // Update is called once per frame
    void Update()
    {
        mimicObject.localPosition = transform.localPosition;
        mimicObject.localRotation = transform.localRotation;
        mimicObject.localScale = transform.localScale;
    }
}
