using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI inst;
    public PlayerHealthbarHUD healthbar;
    public InventoryHUD inventory;

    void Awake() {
        inst = this;
    }
}
