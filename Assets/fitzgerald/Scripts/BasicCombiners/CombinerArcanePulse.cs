using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerArcanePulse : CombinerObject
{
    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "Arcane Pulse",
            display = "2728", // Emoji code for ✨, symbolizing the arcane or magical essence
            code = "public override void Action()\n" +
                   "{\n" +
                   "    float pulseRadius = 4f;\n" +
                   "    float knockbackForce = 500f;\n" +
                   "    Vector3 pulseCenter = transform.position;\n" +
                   "    // Visual effect for arcane pulse\n" +
                   "    GameObject arcaneEffect = Instantiate(particleSystem, pulseCenter, Quaternion.identity);\n" +
                   "    arcaneEffect.GetComponent<ParticleSystem>().startColor = new Color(128, 0, 128); // Deep purple to symbolize arcane magic\n" +
                   "    arcaneEffect.AddComponent<DestroyAfterTime>().lifetime = 1f;\n" +
                   "    Collider[] enemiesAffected = Physics.OverlapSphere(pulseCenter, pulseRadius);\n" +
                   "    foreach (var enemy in enemiesAffected)\n" +
                   "    {\n" +
                   "        if (enemy.gameObject.GetComponent<FitzHealth>())\n" +
                   "        {\n" +
                   "            // Apply knockback effect\n" +
                   "            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();\n" +
                   "            if (enemyRb != null)\n" +
                   "            {\n" +
                   "                Vector3 knockbackDirection = (enemy.transform.position - pulseCenter).normalized;\n" +
                   "                enemyRb.AddForce(knockbackDirection * knockbackForce);\n" +
                   "            }\n" +
                   "            GameObject tempDamageDealer = new GameObject(\"ArcaneDamageDealer\");\n" +
                   "            tempDamageDealer.transform.position = enemy.transform.position;\n" +
                   "            var damageComponent = tempDamageDealer.AddComponent<DamageOnCollision>();\n" +
                   "            damageComponent.damage = 20;\n" +
                   "            Destroy(tempDamageDealer, 0.1f); // Destroy immediately after dealing damage\n" +
                   "        }\n" +
                   "    }\n" +
                   "}\n"
        };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }

    public override void Action()
    {
        float pulseRadius = 4f;
        float knockbackForce = 500f;
        Vector3 pulseCenter = transform.position;
        // Visual effect for arcane pulse
        GameObject arcaneEffect = Instantiate(particleSys, pulseCenter, Quaternion.identity);
        arcaneEffect.GetComponent<ParticleSystem>().startColor = new Color(128, 0, 128); // Deep purple to symbolize arcane magic
        arcaneEffect.AddComponent<DestroyAfterTime>().lifetime = 1f;
        Collider[] enemiesAffected = Physics.OverlapSphere(pulseCenter, pulseRadius);
        foreach (var enemy in enemiesAffected)
        {
            if (enemy.gameObject.GetComponent<FitzHealth>())
            {
                // Apply knockback effect
                Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    Vector3 knockbackDirection = (enemy.transform.position - pulseCenter).normalized;
                    enemyRb.AddForce(knockbackDirection * knockbackForce);
                }
                GameObject tempDamageDealer = new GameObject("ArcaneDamageDealer");
                tempDamageDealer.transform.position = enemy.transform.position;
                var damageComponent = tempDamageDealer.AddComponent<DamageOnCollision>();
                damageComponent.damage = 20;
                Destroy(tempDamageDealer, 0.1f); // Destroy immediately after dealing damage
            }
        }
    }
}
