using UnityEngine;
using static Create;
public class rotateTest : MonoBehaviour
{
    public float rotationSpeed = 50.0f; // Adjust this to control the speed of rotation
    // Update is called once per frame
    void Update()
    {
        // Rotate the object around the y-axis
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
    }
}

