using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GrabItemEvent : UnityEvent<CombinerObject> {}

public class ActiveItem : MonoBehaviour
{
    [SerializeField] float reach;
    [SerializeField] float grabAngle = 90f;
    [SerializeField] KeyCode grabDropButton;
    [SerializeField] KeyCode useButton;
    [SerializeField] KeyCode combineButton;
    [SerializeField] KeyCode grabDropButtonAlt;
    [SerializeField] KeyCode useButtonAlt;
    [SerializeField] KeyCode combineButtonAlt;
    private CombinerObject grabbed;

    public Transform grabbedItemRoot;

    public GameObject selectionRingPrefab;
    private GameObject selectionRingInst;
    public Vector3 selectionRingOffset;

    private CombinerObject currentItemInFront;

    public GrabItemEvent OnGrabItem;
    public GrabItemEvent OnDropItem;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(grabDropButtonAlt)) Debug.Log("Pressed!");
        if (Input.GetKeyDown(grabDropButton) || Input.GetKeyDown(grabDropButtonAlt)) GrabDrop();
        if (Input.GetKeyDown(useButton) || Input.GetKeyDown(useButtonAlt)) Use();
        if (Input.GetKeyDown(combineButton) || Input.GetKeyDown(combineButtonAlt)) Combine();

        currentItemInFront = ItemInFront();
        if (currentItemInFront) {
            if (!selectionRingInst) {
                selectionRingInst = Instantiate(selectionRingPrefab);
            }
            selectionRingInst.transform.position = currentItemInFront.transform.position + selectionRingOffset;
        } else {
            if (selectionRingInst) {
                Destroy(selectionRingInst);
            }
        }
    }

    public void ForceDrop()
    {
        if (!grabbed) return;
        grabbed.transform.parent = null;
        OnDropItem.Invoke(grabbed);
        grabbed = null;
    }

    void GrabDrop()
    {
        if (grabbed)
        {
            ForceDrop();
        }
        else
        {
            grabbed = currentItemInFront;//ItemInFront();
            if (grabbed)
            {
                grabbed.transform.parent = grabbedItemRoot;
                grabbed.transform.localPosition = Vector3.zero;
                grabbed.transform.localRotation = Quaternion.identity;
                OnGrabItem.Invoke(grabbed);
            }
        }
    }

    void Use()
    {
        if (grabbed)
        {
            grabbed.Use();
        }
    }

    void Combine()
    {
        CombinerObject a = currentItemInFront;//ItemInFront();
        if (a != null && grabbed != null)
        {
            CombinerManager.Instance.Combine(a, grabbed);
        }
    }

    CombinerObject ItemInFront()
    {
        var hits = Physics.OverlapSphere(transform.position, reach);
        foreach (var hit in hits) {
            var combiner = hit.gameObject.GetComponent<CombinerObject>();
            // If we didn't hit a combiner, or if it's the one we're currently holding, then skip it
            if (!combiner || (grabbed && grabbed == combiner)) {
                continue;
            }

            if(Mathf.Acos(Vector3.Dot((hit.transform.position - transform.position).normalized, transform.forward)) <= (grabAngle / 2) * Mathf.Deg2Rad) {
                // Debug.Log("Grabbing this object: " + hit.gameObject.name);
                return combiner;
            }
        }
        return null;

        // RaycastHit hit;
        // if (Physics.Raycast(transform.position,transform.forward,out hit, reach))
        // {
        //     return hit.transform.gameObject.GetComponent<CombinerObject>();
        // }
        // return null;
    }

    public bool GrabbingObject() {
        return grabbed;
    }
}
