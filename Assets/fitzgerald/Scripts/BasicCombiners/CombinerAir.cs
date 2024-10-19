using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerAir : CombinerObject
{
    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "Air",
            display = "1F4A8",
            code = "public override void Action()\n" +
                "{\n" +
                "   // swaps places with an enemy if one is standing in front of you\n" +
                    "   RaycastHit hit;\n" +
                    "   if (Physics.SphereCast(transform.position, 1, transform.forward, out hit, 10))\n" +
                    "   {\n" +
                    "       if (hit.transform.gameObject.GetComponent<FitzHealth>())\n" +
                    "       {\n" +
                    "           GameObject airEffect = Instantiate(particleSys); //if an enemy is in front of us, swap the player and enemy position\n" +
                    "           Vector3 oldPos = player.position;\n" +
                    "           airEffect.transform.position = oldPos;\n" +
                    "           var main = airEffect.GetComponent<ParticleSystem>().main;\n" +
                    "           main.startColor = Color.white;\n" +
                    "          airEffect.AddComponent<DestroyAfterTime>().lifetime = 1;\n" +
                    "          Debug.Log($\"at position {player.position} moving to {hit.transform.position}\");\n" +
                    "          player.position = hit.transform.position;\n" +
                    "          hit.transform.position = oldPos;\n" +
                    "      }\n" +
                    "   }" +
                "}\n"
                
    };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }

    public override void Action()
    {
        // swaps places with an enemy if one if standing in front of you
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 1, transform.forward, out hit, 10))
        {
            if (hit.transform.gameObject.GetComponent<FitzEnemy>())
            {
                GameObject airEffect = Instantiate(particleSys);
                Vector3 oldPos = player.position;
                airEffect.transform.position = oldPos;
                var main = airEffect.GetComponent<ParticleSystem>().main;
                main.startColor = Color.white;
                airEffect.AddComponent<DestroyAfterTime>().lifetime = 1;
                Debug.Log($"at position {player.position} moving to {hit.transform.position}");
                player.position = hit.transform.position;
                hit.transform.position = oldPos;
            }
        }
    }
}
