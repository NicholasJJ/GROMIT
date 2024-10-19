using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerEarthquake : CombinerObject
{
    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "Earthquake",
            display = "1F4A9", // Emoji code for a stylized representation of shaking ground
            code = "public override void Action()\n" +
                   "{\n" +
                   "    // Creates an area effect that simulates earthquake tremors\n" +
                   "    float effectRadius = 5f;\n" +
                   "    Vector3 effectCenter = transform.position;\n" +
                   "    GameObject earthquakeEffect = Instantiate(particleSys, effectCenter, Quaternion.identity);\n" +
                   "    earthquakeEffect.GetComponent<ParticleSystem>().startColor = new Color(139, 69, 19); // Brown color\n" +
                   "    earthquakeEffect.AddComponent<DestroyAfterTime>().lifetime = 3f;\n" +
                   "    Collider[] affectedColliders = Physics.OverlapSphere(effectCenter, effectRadius);\n" +
                   "    foreach (var collider in affectedColliders)\n" +
                   "    {\n" +
                   "        GameObject tempDamageDealer = new GameObject(\"EarthquakeDamageDealer\");\n" +
                   "        tempDamageDealer.transform.position = collider.transform.position;\n" +
                   "        var damageComponent = tempDamageDealer.AddComponent<DamageOnCollision>();\n" +
                   "        damageComponent.damage = 10;\n" +
                   "        Destroy(tempDamageDealer, 0.5f); // Destroy the temporary object shortly after creation\n" +
                   "    }\n" +
                   "}\n"
        };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }

    public override void Action()
    {
        // Creates an area effect that simulates earthquake tremors
        float effectRadius = 5f;
        Vector3 effectCenter = transform.position;
        GameObject earthquakeEffect = Instantiate(particleSys, effectCenter, Quaternion.identity);
        earthquakeEffect.GetComponent<ParticleSystem>().startColor = new Color(139, 69, 19); // Brown color
        earthquakeEffect.AddComponent<DestroyAfterTime>().lifetime = 3f;
        Collider[] affectedColliders = Physics.OverlapSphere(effectCenter, effectRadius);
        foreach (var collider in affectedColliders)
        {
            GameObject tempDamageDealer = new GameObject("EarthquakeDamageDealer");
            tempDamageDealer.transform.position = collider.transform.position;
            var damageComponent = tempDamageDealer.AddComponent<DamageOnCollision>();
            damageComponent.damage = 10;
            Destroy(tempDamageDealer, 0.5f); // Destroy the temporary object shortly after creation
        }
    }
}
