using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerThunder : CombinerObject
{
    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "Thunder",
            display = "26A1", // Emoji code for ⚡
            code = "public override void Action()\n" +
                   "{\n" +
                   "    // Targets a point directly in front of the player and summons a thunder strike\n" +
                   "    Vector3 targetPosition = transform.position + transform.forward * 5;\n" +
                   "    GameObject thunderEffect = Instantiate(particleSystem, targetPosition, Quaternion.identity);\n" +
                   "    thunderEffect.GetComponent<ParticleSystem>().startColor = Color.yellow;\n" +
                   "    thunderEffect.AddComponent<DestroyAfterTime>().lifetime = 0.5f;\n" +
                   "    Collider[] hitColliders = Physics.OverlapSphere(targetPosition, 3);\n" +
                   "    foreach (var hitCollider in hitColliders)\n" +
                   "    {\n" +
                   "        if (hitCollider.gameObject.GetComponent<FitzHealth>())\n" +
                   "        {\n" +
                   "            // Apply damage and potentially a stun effect\n" +
                   "            hitCollider.gameObject.GetComponent<FitzHealth>().ApplyDamage(20);\n" +
                   "            // Optional: Implement a method to stun the enemy\n" +
                   "            // hitCollider.gameObject.GetComponent<EnemyController>().Stun(2);\n" +
                   "        }\n" +
                   "    }\n" +
                   "}\n"
        };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }

    public override void Action()
    {
        // Targets a point directly in front of the player and summons a thunder strike
        Vector3 targetPosition = transform.position + transform.forward * 5;
        GameObject thunderEffect = Instantiate(particleSys, targetPosition, Quaternion.identity);
        var main = thunderEffect.GetComponent<ParticleSystem>().main;
        main.startColor = Color.yellow;
        thunderEffect.AddComponent<DestroyAfterTime>().lifetime = 0.5f;
        thunderEffect.AddComponent<SphereCollider>().center = Vector3.zero;
        thunderEffect.GetComponent<SphereCollider>().isTrigger = true;
        thunderEffect.GetComponent<SphereCollider>().radius = 1;
        thunderEffect.AddComponent<DamageOnCollision>().damage = 30;
    }
}
