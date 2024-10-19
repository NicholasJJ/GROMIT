using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManager : MonoBehaviour
{
    // Which gate is UP
    public enum GateState {
        Red,
        Blue
    }

    private static GateManager _inst;
    public static GateManager inst {
        get { return _inst; }
    }

    private GateState state = GateState.Blue;
    private List<GateButton> buttons = new List<GateButton>();
    private List<Gate> gates = new List<Gate>();

    public Color colorRed;
    public Color colorBlue;

    void Awake() {
        _inst = this;
    }

    public void RegisterGate(Gate gate) {
        gates.Add(gate);
        gate.SetState(state);
    }

    public void RegisterButton(GateButton button) {
        buttons.Add(button);
        button.SetState(state);
    }

    public void ToggleGates() {
        // Debug.Log("Toggle Gates");
        if (state == GateState.Red) {
            state = GateState.Blue;
        } else if (state == GateState.Blue) {
            state = GateState.Red;
        }

        foreach (var button in buttons) {
            button.SetState(state);
        }

        foreach (var gate in gates) {
            gate.SetState(state);
        }
    }

    public Color GetStateColor(GateState checkState) {
        if (checkState == GateState.Red) {
            return colorRed;
        }

        if (checkState == GateState.Blue) {
            return colorBlue;
        }

        return Color.white;
    }
}
