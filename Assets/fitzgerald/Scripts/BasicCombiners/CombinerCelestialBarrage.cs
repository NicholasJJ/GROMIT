using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerCelestialBarrage : CombinerObject
{
    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "Celestial Barrage",
            display = "2604", // Emoji code for ☄, representing the celestial or meteoric essence
            code = "public override void Action()\n" +
                   "{\n" +
                   "    int numberOfProjectiles = 5;\n" +
                   "    float radius = 10f;\n" +
                   "    Vector3 attackCenter = transform.position;\n" +
                   "    for (int i = 0; i < numberOfProjectiles; i++)\n" +
                   "    {\n" +
                   "        Vector3 spawnPosition = attackCenter + new Vector3(Random.Range(-radius, radius), 10, Random.Range(-radius, radius));\n" +
                   "        GameObject projectile = Instantiate(particleSystem, spawnPosition, Quaternion.identity);\n" +
                   "        projectile.GetComponent<ParticleSystem>().startColor = new Color(255, 165, 0); // Orange color for a fiery effect\n" +
                   "        projectile.AddComponent<DestroyAfterTime>().lifetime = 2f;\n" +
                   "        // Assuming projectile has a method to move towards the ground and deal damage upon collision\n" +
                   "        Rigidbody rb = projectile.AddComponent<Rigidbody>();\n" +
                   "        rb.useGravity = true;\n" +
                   "        var damageComponent = projectile.AddComponent<DamageOnCollision>();\n" +
                   "        damageComponent.damage = 20;\n" +
                   "        // Optional: Add a component to make the projectile home in on nearby enemies or explode on impact\n" +
                   "    }\n" +
                   "}\n"
        };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }

    public override void Action()
    {
        int numberOfProjectiles = 5;
        float radius = 10f;
        Vector3 attackCenter = transform.position;
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Vector3 spawnPosition = attackCenter + new Vector3(Random.Range(-radius, radius), 10, Random.Range(-radius, radius));
            GameObject projectile = Instantiate(particleSys, spawnPosition, Quaternion.identity);
            projectile.GetComponent<ParticleSystem>().startColor = new Color(255, 165, 0); // Orange color for a fiery effect
            projectile.AddComponent<DestroyAfterTime>().lifetime = 2f;
            // Assuming projectile has a method to move towards the ground and deal damage upon collision
            Rigidbody rb = projectile.AddComponent<Rigidbody>();
            rb.useGravity = true;
            var damageComponent = projectile.AddComponent<DamageOnCollision>();
            damageComponent.damage = 20;
            // Optional: Add a component to make the projectile home in on nearby enemies or explode on impact
        }
    }
}
