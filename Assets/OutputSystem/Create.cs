using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

public enum Direction
{
    Above,
    Below,
    North,
    South,
    East,
    West
}


public class Create : MonoBehaviour
{
    static Dictionary<Direction, Vector3> directions = new Dictionary<Direction, Vector3> {
        {Direction.Above , Vector3.up},
        {Direction.Below , Vector3.down},
        {Direction.North , Vector3.forward},
        {Direction.South , Vector3.right},
        {Direction.East , Vector3.back},
        {Direction.West , Vector3.left},
    };

    public static GameObject MakeObject(PrimitiveType primitive, Vector3 scale, Vector3 rotation, GameObject relativeTo, Direction direction, float distance, Transform parent = null, bool inSceneGraph = false, string name = "", string description = "")
    {
        GameObject obj = GameObject.CreatePrimitive(primitive);
        obj.transform.localScale = scale;
        obj.transform.rotation = Quaternion.Euler(rotation);
        if (inSceneGraph)
        {
            var textDescription = obj.AddComponent<TextDescription>();
            textDescription.textName = name;
            textDescription.description = description;
        }
        if (name != "")
        {
            obj.name = name;
        }
        PlaceObject(obj.transform, relativeTo, direction, distance, parent);
        return obj;
    }

    static void PlaceObject(Transform original, GameObject sceneBase, Direction dir, float distance, Transform parent = null)
    {
        Transform parentTransform = parent;
        original.position = sceneBase == null ? Vector3.zero : sceneBase.transform.position;
        original.position += directions[dir] * distance;
        original.parent = parentTransform;
    }

    //void t()
    //{
    //    GameObject cup = GameObject.Find("Cup");
    //    GameObject cube = MakeObject(PrimitiveType.Cube, new Vector3(1f, 1f, 1f), Vector3.zero, cup.name, Direction.Above, 1f, cup.transform, true, "Cube", "A cube on top of the cup");

    //}
}

//public static class MonoExtension
//{
//    public static Dictionary<Direction, Vector3> directions = new Dictionary<Direction, Vector3> {
//        {Direction.Above , Vector3.up},
//        {Direction.Below , Vector3.down},
//        {Direction.North , Vector3.forward},
//        {Direction.South , Vector3.right},
//        {Direction.East , Vector3.back},
//        {Direction.West , Vector3.left},
//    };
//}
