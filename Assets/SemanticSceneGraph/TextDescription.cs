using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MovementLevel {
    Stationary,
    Movable,
    Moving
}

public class TextDescription : MonoBehaviour
{
    [SerializeField] public MovementLevel movementLevel = MovementLevel.Stationary;
    public string textName;
    [SerializeField] public string description;
    private bool added = false;

    private Vector3 centerCache;
    private Vector3 sizeCache;
    void Start() {
        // Add null check to resolve the conflict with AutoTagger
        TextTree.Instance.AddToDescriptionList(this,ParentCount());
    }

    public void AddToTree() {
        if (added) return;
        added = true;
        /*if (scripts != null)
        {
            scripts = gameObject.GetComponents<MonoBehaviour>();
        }*/
        List<Renderer> renderers = GetComponentsInChildren<Renderer>().ToList();

        if (GetComponent<Renderer>())
            renderers.Add(GetComponent<Renderer>());

        if (renderers.Count > 0)
        {
            // Create an initial bounds that encapsulates the first renderer
            Bounds combinedBounds = renderers[0].bounds;

            // Iterate through the remaining renderers and expand the combined bounds
            for (int i = 1; i < renderers.Count; i++)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            // Get the center and size of the combined bounding box
            centerCache = combinedBounds.center;
            sizeCache = combinedBounds.size;

            TextTree.Instance.AddNode(this,GetName(),movementLevel == MovementLevel.Moving || movementLevel == MovementLevel.Movable);
        }
        else
        {
            Debug.Log("No renderers found! object is " + gameObject.name);
            centerCache = transform.position;
            sizeCache = Vector3.zero;

            TextTree.Instance.AddNode(this, GetName(), movementLevel == MovementLevel.Moving || movementLevel == MovementLevel.Movable);
        }
    }

    public void UpdateTree(){
        if (!added) {AddToTree(); return;}
        TextTree.Instance.UpdateNode(this);
    }

    public string GetName() => string.IsNullOrEmpty(textName) ? gameObject.name : textName;
    public string GetDescription() => description;
    public string[] GetScripts() {
        MonoBehaviour[] scripts = gameObject.GetComponents<MonoBehaviour>();
        //string[] scriptNames = new string[scripts.Length];
        List<string> scriptNames = new List<string>();

        for (int i = 0; i < scripts.Length; i++)
        {
            if (scripts[i] != null && scripts[i].GetType().Name.Length > 0)
            {
                scriptNames.Add(scripts[i].GetType().Name);
            }
        }
        return scriptNames.ToArray();
    }

    public (Vector3,Vector3) GetCenterAndSize() {
        if (movementLevel == MovementLevel.Stationary) return (centerCache,sizeCache);
        List<Renderer> renderers = GetComponentsInChildren<Renderer>().ToList();

        if (GetComponent<Renderer>())
            renderers.Add(GetComponent<Renderer>());

        if (renderers.Count > 0)
        {
            // Create an initial bounds that encapsulates the first renderer
            Bounds combinedBounds = renderers[0].bounds;

            // Iterate through the remaining renderers and expand the combined bounds
            for (int i = 1; i < renderers.Count; i++)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            // Get the center and size of the combined bounding box
            Vector3 center = combinedBounds.center;
            Vector3 size = combinedBounds.size;

            return (center,size);
        }
        else 
        {
            return (transform.position,Vector3.zero);
        }
    }

    private int ParentCount() {
        int count = 0;
        Transform t = transform;
        while (t != null) {
            count++;
            t = t.parent;
        }
        return count;
    }
}
