using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateButton : MonoBehaviour
{
    public Renderer graphics;

    void Start() {
        if (GateManager.inst) {
            var health = GetComponent<FitzHealth>();
            if (health) {
                health.OnTakeDamage.AddListener((dmg, causer) => {
                    GateManager.inst.ToggleGates();
                });
            }

            GateManager.inst.RegisterButton(this);
        }

        if (!graphics) {
            graphics = GetComponent<Renderer>();
        }
    }

    public void SetState(GateManager.GateState state) {
        if (graphics) {
            graphics.material.color = GateManager.inst.GetStateColor(state);
        }
    }
}
