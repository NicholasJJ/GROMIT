using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitzDoor : MonoBehaviour
{
    public string keyItemName;
    public List<GameObject> doorObjects = new List<GameObject>();

    public void DoorOpen() {
        // gameObject.SetActive(false);
        foreach (var doorObj in doorObjects) {
            doorObj.SetActive(false);
        }
        GetComponent<BoxCollider>().enabled = false;
    }

    public void DoorClose() {
        // gameObject.SetActive(true);
        foreach (var doorObj in doorObjects) {
            doorObj.SetActive(true);
        }
        GetComponent<BoxCollider>().enabled = true;
    }

    public void CheckDoorCollision(GameObject collider) {
        Debug.Log($"Collision with {collider.name}");

        var inventory = collider.GetComponent<FitzInventory>();
        if (inventory && inventory.HasItem(keyItemName)) {
            DoorOpen();
            inventory.UseItem(keyItemName);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // We won't actually use this because we use the collision in character controller
        CheckDoorCollision(collision.collider.gameObject);
    }
}
