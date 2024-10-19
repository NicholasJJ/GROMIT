using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerWater : CombinerObject
{
    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "Water",
            display = "1F4A7",
            code = "public override void Action()\n" +
                "{\n" +
                "    // Shoots a water projectile\n" +
                    "    GameObject water = GameObject.CreatePrimitive(PrimitiveType.Sphere);\n" +
                    "    water.transform.localScale = 0.3f * Vector3.one;\n" +
                    "    water.GetComponent<Renderer>().material.color = Color.blue;\n" +
                    "    water.transform.position = transform.position + transform.forward;\n" +
                    "    water.AddComponent<DamageOnCollision>().damage = 15;\n" +
                    "    water.AddComponent<Rigidbody>().AddForce(transform.forward * 1000);\n" +
                    "    water.AddComponent<DestroyAfterTime>().lifetime = 1;\n" +
                "}\n"
        };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }


    public override void Action()
    {
        //shoots a water projectile
        GameObject water = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        water.transform.localScale = 0.3f * Vector3.one;
        water.GetComponent<Renderer>().material.color = Color.blue;
        water.transform.position = transform.position + transform.forward;
        water.AddComponent<DamageOnCollision>().damage = 15;
        water.AddComponent<Rigidbody>().AddForce(transform.forward * 1000);
        water.AddComponent<DestroyAfterTime>().lifetime = 1;
    }
}
