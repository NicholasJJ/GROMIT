using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        // Ensure that the canvas faces the main camera
        transform.LookAt(Camera.main.transform.position);
        transform.forward = -transform.forward;
    }
}
