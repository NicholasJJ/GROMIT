using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitzInventoryItem : MonoBehaviour
{
    public string itemName;

    public void CheckPickup(GameObject collider) {
        Debug.Log($"Collision with {collider.name}");

        var inventory = collider.GetComponent<FitzInventory>();
        if (inventory) {
            inventory.AddItem(this);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckPickup(collision.collider.gameObject);
    }
}
