using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveAndRotate : MonoBehaviour
{
    [SerializeField] Vector3 move;
    [SerializeField] Vector3 rot;
    [SerializeField] Space rotSpace = Space.World;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(move * Time.deltaTime);
        transform.Rotate(rot * Time.deltaTime, rotSpace);
    }
}
