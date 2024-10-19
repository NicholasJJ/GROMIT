using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    [SerializeField] Transform handTransform;
    [SerializeField] Transform head;
    [SerializeField] float reach;

    [SerializeField] Transform held;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (!handTransform.GetComponent<Grabber>().IsGrabbing()) {
                Debug.Log("mouse down");
                RaycastHit hit;
                if (Physics.Raycast(head.position,head.forward,out hit,reach)) {
                    Debug.Log("hit " + hit.collider.gameObject.name);
                    GrabableObject grabable = hit.transform.gameObject.GetComponent<GrabableObject>();
                    if (grabable != null) {
                        grabable.transform.position = handTransform.position;
                        handTransform.GetComponent<Grabber>().Grab(grabable.gameObject);
                        held = grabable.transform;
                    }
                }
            }
            else
            {
                RaycastHit hit;
                handTransform.GetComponent<Grabber>().drop();
                if (Physics.Raycast(head.position,head.forward,out hit,reach)) {
                    held.position = hit.point - (head.forward.normalized * 0.1f);
                } else {
                    held.position = head.position + (head.forward * reach);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.E)
            && held != null && held.GetComponent<RuntimeInteractionObject>())
        {
            string debug = $"attempting to interact using {held.GetComponent<RuntimeInteractionObject>().GetID()}";
            RaycastHit hit;
            if (Physics.Raycast(head.position, head.forward, out hit, reach))
            {
                debug += ", hit an object";
                if (hit.collider.gameObject.GetComponent<RuntimeInteractionObject>())
                {
                    debug += ", object had a interaction script";
                    held.GetComponent<RuntimeInteractionObject>().Interact(hit.collider.gameObject.GetComponent<RuntimeInteractionObject>());
                }
            }
            Debug.Log(debug);
        }
    }
}
