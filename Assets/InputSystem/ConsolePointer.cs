using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsolePointer : InputSource
{
    [SerializeField] KeyCode pointKey;
    [SerializeField] Transform cam;
    [SerializeField] GameObject indicator;
    private List<GameObject> indicators;
    public override void EndRecord()
    {
        requestObject.CloseChannel();
        foreach (var ind in indicators) {
            Destroy(ind);
        }
        indicators = new List<GameObject>();
    }

    protected override void SetupRecord()
    {
        indicators = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pointKey) && InputManager.Instance.IsRecording()) {
            RaycastHit hit;
            if (Physics.Raycast(cam.position,cam.forward,out hit)) {
                Debug.Log("hit object: " + hit.collider.gameObject.name);
                var description = hit.collider.gameObject.GetComponentInParent<TextDescription>();
                if (description != null) {
                    Debug.Log("requestObject= " + requestObject);
                    Debug.Log("description= " + description.GetName());
                    requestObject.PointAt(description,Time.time);
                    var centerAndSize = description.GetCenterAndSize();
                    var ind = Instantiate(indicator,centerAndSize.Item1,Quaternion.identity);
                    ind.transform.localScale = centerAndSize.Item2 * 1.5f;
                    indicators.Add(ind);
                }
            }
        }
    }
}
