using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class GrabableObject : MonoBehaviour
{
    [SerializeField] Transform grabbedBy;
    private Rigidbody rBody;

    Dictionary<Rigidbody,FixedJoint> joints = new Dictionary<Rigidbody, FixedJoint>();

    public UnityEvent onGrabOrDrop;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        if (grabbedBy) Grab(grabbedBy.GetComponent<Rigidbody>());
    }

    public void Grab(Rigidbody grabber) {
        onGrabOrDrop.Invoke();
        if (joints.ContainsKey(grabber)) return;
        joints[grabber] = gameObject.AddComponent<FixedJoint>();
        joints[grabber].connectedBody = grabber;
        joints[grabber].breakForce = float.PositiveInfinity;
        joints[grabber].breakTorque = float.PositiveInfinity;
        joints[grabber].connectedMassScale = 1;
        joints[grabber].massScale = 1;
        joints[grabber].enableCollision = false;
        joints[grabber].enablePreprocessing = false;
    }

    public void Drop(Rigidbody dropper) {
        onGrabOrDrop.Invoke();
        if (!joints.ContainsKey(dropper)) return;
        Destroy(joints[dropper]);
        joints.Remove(dropper);
    }
}
