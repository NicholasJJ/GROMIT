using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthbarHUD : MonoBehaviour
{
    public Slider healthbar;
    public GameObject restartNote;

    // Health bar is percentage based (0,1) - Health asset determines min/max
    public void SetHealth(float healthScale) {
        if (healthbar) {
            healthbar.value = healthScale;
            if (healthbar.value <= 0)
            {
                restartNote.SetActive(true);
            }
        }
    }
}
