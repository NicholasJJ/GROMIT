using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerNaturesWrath : CombinerObject
{
    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "Nature's Wrath",
            display = "1F333", // Emoji code for 🌳, symbolizing nature's power
            code = "public override void Action()\n" +
                   "{\n" +
                   "    float radius = 5f;\n" +
                   "    Vector3 center = transform.position;\n" +
                   "    // Visual effect for vines growing\n" +
                   "    GameObject vinesEffect = Instantiate(particleSystem, center, Quaternion.identity);\n" +
                   "    vinesEffect.GetComponent<ParticleSystem>().startColor = Color.green;\n" +
                   "    vinesEffect.AddComponent<DestroyAfterTime>().lifetime = 3f;\n" +
                   "    Collider[] enemiesInRadius = Physics.OverlapSphere(center, radius);\n" +
                   "    foreach (var enemy in enemiesInRadius)\n" +
                   "    {\n" +
                   "        if (enemy.gameObject.GetComponent<FitzHealth>())\n" +
                   "        {\n" +
                   "            GameObject tempDamageDealer = new GameObject(\"NatureDamageDealer\");\n" +
                   "            tempDamageDealer.transform.position = enemy.transform.position;\n" +
                   "            var damageComponent = tempDamageDealer.AddComponent<DamageOnCollision>();\n" +
                   "            damageComponent.damage = 15;\n" +
                   "            Destroy(tempDamageDealer, 3f); // Lasts as long as the vines\n" +
                   "        }\n" +
                   "    }\n" +
                   "}\n"
        };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }

    public override void Action()
    {
        float radius = 5f;
        Vector3 center = transform.position;
        // Visual effect for vines growing
        GameObject vinesEffect = Instantiate(particleSys, center, Quaternion.identity);
        vinesEffect.GetComponent<ParticleSystem>().startColor = Color.green;
        vinesEffect.AddComponent<DestroyAfterTime>().lifetime = 3f;
        Collider[] enemiesInRadius = Physics.OverlapSphere(center, radius);
        foreach (var enemy in enemiesInRadius)
        {
            if (enemy.gameObject.GetComponent<FitzHealth>())
            {
                GameObject tempDamageDealer = new GameObject("NatureDamageDealer");
                tempDamageDealer.transform.position = enemy.transform.position;
                var damageComponent = tempDamageDealer.AddComponent<DamageOnCollision>();
                damageComponent.damage = 15;
                Destroy(tempDamageDealer, 3f); // Lasts as long as the vines, representing sustained damage
            }
        }
    }
}
