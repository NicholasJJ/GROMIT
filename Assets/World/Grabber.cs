using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabber : MonoBehaviour
{
    [SerializeField] float grabRange = 0.05f;
    private bool grabbing = false;
    private FixedJoint joint;
    //1 == success, -1 == object is not grabbable, 0 == out of reach
    public int Grab(GameObject target) {
        if (!target.GetComponent<GrabableObject>()) return -1;
        if (grabRange < Vector3.Distance(target.GetComponent<Collider>()
                                        .ClosestPoint(transform.position),transform.position)) {
                                            Debug.Log("Distance is " + Vector3.Distance(target.GetComponent<Collider>()
                                        .ClosestPoint(transform.position),transform.position));
                                            return 0;
                                        }
        if (joint == null) joint = gameObject.AddComponent<FixedJoint>();
        if (joint.connectedBody != null) drop();
        joint.connectedBody = target.GetComponent<Rigidbody>();
        joint.breakForce = float.PositiveInfinity;
        joint.breakTorque = float.PositiveInfinity;
        joint.connectedMassScale = 1;
        joint.massScale = 1;
        joint.enableCollision = false;
        joint.enablePreprocessing = false;
        target.GetComponent<GrabableObject>().Grab(gameObject.GetComponent<Rigidbody>());
        grabbing = true;
        return 1;
    }

    public void drop() {
        if (joint.connectedBody != null && joint.connectedBody.gameObject.GetComponent<GrabableObject>())
            joint.connectedBody.gameObject.GetComponent<GrabableObject>().Drop(gameObject.GetComponent<Rigidbody>());
        joint.connectedBody = null;
        grabbing = false;
    }

    public bool IsGrabbing() {
        return grabbing;
    }
}
