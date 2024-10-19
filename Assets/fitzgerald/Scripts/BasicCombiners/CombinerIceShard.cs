using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerIceShard : CombinerObject
{
    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "Ice Shard",
            display = "2744", // Emoji code for ❄️
            code = "public override void Action()\n" +
                   "{\n" +
                   "    // Launches 3 ice shards in front of the player\n" +
                   "    int numberOfShards = 3;\n" +
                   "    float spreadAngle = 10f;\n" +
                   "    for (int i = 0; i < numberOfShards; i++)\n" +
                   "    {\n" +
                   "        GameObject iceShard = GameObject.CreatePrimitive(PrimitiveType.Cube);\n" +
                   "        iceShard.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f);\n" +
                   "        iceShard.GetComponent<Renderer>().material.color = Color.cyan;\n" +
                   "        iceShard.transform.position = transform.position + transform.forward * 1f + transform.right * (i - 1) * spreadAngle / 100;\n" +
                   "        iceShard.AddComponent<Rigidbody>().useGravity = false;\n" +
                   "        iceShard.GetComponent<Rigidbody>().AddForce(transform.forward * 1000 + transform.right * (i - 1) * spreadAngle);\n" +
                   "        var damageComponent = iceShard.AddComponent<DamageOnCollision>();\n" +
                   "        damageComponent.damage = 15;\n" +
                   "        Destroy(iceShard, 2f); // Destroy the ice shard after 2 seconds\n" +
                   "    }\n" +
                   "}\n"
        };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }

    public override void Action()
    {
        // Launches 3 ice shards in front of the player
        int numberOfShards = 3;
        float spreadAngle = 10f;
        for (int i = 0; i < numberOfShards; i++)
        {
            GameObject iceShard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            iceShard.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f);
            iceShard.GetComponent<Renderer>().material.color = Color.cyan;
            iceShard.transform.position = transform.position + transform.forward * 1f + transform.right * (i - 1) * spreadAngle / 100;
            iceShard.AddComponent<Rigidbody>().useGravity = false;
            iceShard.GetComponent<Rigidbody>().AddForce(transform.forward * 1000 + transform.right * (i - 1) * spreadAngle);
            var damageComponent = iceShard.AddComponent<DamageOnCollision>();
            damageComponent.damage = 15;
            Destroy(iceShard, 2f); // Destroy the ice shard after 2 seconds
        }
    }
}
