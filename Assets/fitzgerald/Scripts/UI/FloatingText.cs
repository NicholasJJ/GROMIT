using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] float floatSpeed = 2.0f;
    [SerializeField] float floatTime = 1.0f;
    [SerializeField] Vector3 floatDirection = Vector3.up;

    private float spawnTime;
    private TMPro.TMP_Text textObject;

    void Start()
    {
        textObject = GetComponent<TMPro.TMP_Text>();
        spawnTime = Time.time;
    }

    void Update()
    {
        transform.position += floatDirection * floatSpeed * Time.deltaTime;
        if (Time.time > spawnTime + floatTime)
        {
            Destroy(gameObject);
        }
    }
}
