using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyInteraction : RuntimeInteractionObject
{
    [SerializeField] RuntimeInteractionObject Door;

    public static void OpenDoor(GameObject key, GameObject door)
    {
        GameObject.Destroy(door);
        GameObject.Destroy(key);
    }
    string openDoorString = "public static void OpenDoor(GameObject key, GameObject door) \n{ \nGameObject.Destroy(door); \nGameObject.Destroy(key); \n}";

    public override Dictionary<RuntimeInteractionObject, Interaction> DevMadeInteractions()
    {
        return new Dictionary<RuntimeInteractionObject, Interaction>
        {
            {Door , new Interaction("Opens the door with the key", openDoorString,OpenDoor)}
        };
    }
}
