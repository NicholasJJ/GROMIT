using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputSource : MonoBehaviour
{
    protected InputRequest requestObject;
    private void Start() {
        InputManager.Instance.AddSource(this);
    }

    public void StartRecord(InputRequest requestObject) {
        this.requestObject = requestObject;
        SetupRecord();
    }
    protected abstract void SetupRecord();
    public abstract void EndRecord();
}
