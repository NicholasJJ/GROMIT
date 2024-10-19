using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(TextDescription))]
public class RuntimeInteractionObject : MonoBehaviour
{
    [SerializeField] string id = "";

    // Start is called before the first frame update
    void Awake()
    {
        //RuntimeInteractionMaker.Instance.AddObject(this);
        FindObjectOfType<RuntimeInteractionMaker>().AddObject(this);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    var interactionObj = collision.gameObject.GetComponent<RuntimeInteractionObject>();
    //    if (interactionObj)
    //    {
    //        RuntimeInteractionMaker.Instance.Interact(this, interactionObj);
    //    }
    //}

    public string GetID() => id.Equals("") ? gameObject.name : id;

    public void Interact(RuntimeInteractionObject other) {
        Debug.Log("Interacting");
        RuntimeInteractionMaker.Instance.Interact(this, other);
        RuntimeInteractionMaker.Instance.Interact(other, this);
    } 

    public virtual Dictionary<RuntimeInteractionObject, Interaction> DevMadeInteractions()
        => new Dictionary<RuntimeInteractionObject, Interaction>();

    public string GetDescription() => GetComponent<TextDescription>().description;
}
