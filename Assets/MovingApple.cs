using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingApple : MonoBehaviour
{
    public float jitterSpeed = 2f;   // Speed of the jitter
    public float jitterDistance = 0.1f; // Distance the apple will jitter up and down

    private Vector3 startPosition;

    void Start()
    {
        // Store the initial position of the apple
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the vertical offset using a sine function for smooth oscillation
        float yOffset = Mathf.Sin(Time.time * jitterSpeed) * jitterDistance;

        // Apply the offset to the apple's position
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);
    }
}
