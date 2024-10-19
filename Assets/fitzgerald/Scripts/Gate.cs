using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GateManager.GateState gateType;

    public Renderer graphics;

    public Transform animateTform;
    public float upYOffset = 0.5f;
    public float downYOffset = -1.0f;
    public float transitionTime = 0.25f;

    void Start() {
        if (GateManager.inst) {
            GateManager.inst.RegisterGate(this);

            if (!graphics) {
                graphics = GetComponent<Renderer>();
            }
            if (graphics) {
                graphics.material.color = GateManager.inst.GetStateColor(gateType);
            }
        }
    }

    public void SetState(GateManager.GateState state) {
        if (gateType == state) {
            float correctedTime = (animateTform.position.y - upYOffset) / (downYOffset - upYOffset) * transitionTime;
            LeanTween.moveY(animateTform.gameObject, upYOffset, correctedTime);
        } else {
            float correctedTime = (animateTform.position.y - downYOffset) / (upYOffset - downYOffset) * transitionTime;
            LeanTween.moveY(animateTform.gameObject, downYOffset, correctedTime);
        }
    }
}
