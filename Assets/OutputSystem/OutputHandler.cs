using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OutputHandler : MonoBehaviour
{
    public void Start() {
        OutputManager.Instance.AddHandler(this);
    }
    public abstract void HandleOutput(Output output);
}
