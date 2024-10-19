using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    Rigidbody rBody;
    [SerializeField] float speed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float jumpForce;
    GameObject head;
    Vector2 mouseLook = Vector2.zero;
    [SerializeField] private bool inAir;


    [SerializeField] float gravity = 9.8f;
    // Start is called before the first frame update
    void Start()
    {
        rBody = gameObject.GetComponent<Rigidbody>();
        head = GameObject.FindGameObjectWithTag("MainCamera");
        LockMouse();
        inAir = true;
    }

    // Update is called once per frame
    void Update()
    {

        //Movement
        //transform.Translate(move);
        Vector3 v = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        if (v.magnitude > 1) v.Normalize();
        v *= speed;
        // rBody.velocity = new Vector3(0, rBody.velocity.y, 0);
        rBody.velocity = v;

        //Jumping
        if (Input.GetButtonDown("Jump") && !inAir)
        {
            inAir = true;
            rBody.AddForce(Vector3.up * jumpForce);
        }

        //Looking
        float prevX = mouseLook.x;
        mouseLook += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouseLook = new Vector2(mouseLook.x, Mathf.Clamp(mouseLook.y, -90 / mouseSensitivity, 90 / mouseSensitivity));
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            transform.localRotation = Quaternion.AngleAxis((mouseLook.x - prevX) * mouseSensitivity, Vector3.up) * transform.localRotation;
            head.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y * mouseSensitivity, Vector3.right);
        }

        //Mouse
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (Cursor.visible)
                LockMouse();
            else
                UnlockMouse();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        inAir = false;
    }

    public void LockMouse()
    {
        // Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
