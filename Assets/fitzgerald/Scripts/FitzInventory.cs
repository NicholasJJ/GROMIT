using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

public class FitzInventory : MonoBehaviour
{
    public List<FitzInventoryItem> inventoryItems = new List<FitzInventoryItem>();
    public Dictionary<string, int> itemCount = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem(FitzInventoryItem inventoryItem) {
        inventoryItems.Add(inventoryItem);

        var itemName = inventoryItem.itemName;
        if (!itemCount.ContainsKey(itemName)) {
            itemCount.Add(itemName, 0);
        }
        itemCount[itemName] += 1;

        PlayerUI.inst.inventory.UpdateItemCount(itemName, itemCount[itemName]);
    }

    public bool HasItem(string itemName) {
        if (itemCount.ContainsKey(itemName) && itemCount[itemName] > 0) {
            return true;
        }
        return false;
        // foreach (var item in inventoryItems) {
        //     if (item.itemName == itemName) {
        //         return true;
        //     }
        // }
        // return false;
    }

    public void UseItem(string itemName) {
        itemCount[itemName] -= 1;
        PlayerUI.inst.inventory.UpdateItemCount(itemName, itemCount[itemName]);
    }
}
