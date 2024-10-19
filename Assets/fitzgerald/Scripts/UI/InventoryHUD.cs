using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryHUD : MonoBehaviour
{
    [System.Serializable]
    public struct ItemUIBinding {
        public string name;
        public TMP_Text countText;
    }

    public List<ItemUIBinding> uiBindings;

    public void UpdateItemCount(string name, int count) {
        foreach (var binding in uiBindings) {
            if (binding.name == name) {
                binding.countText.text = $"{count}";
            }
        }
    }
}
