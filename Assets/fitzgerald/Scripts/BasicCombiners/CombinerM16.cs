using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerM16 : CombinerObject
{
    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "M16",
            display = "1F52B", // Emoji code for 🔫, representing firearms
            code = "public override void Action()\n" +
                   "{\n" +
                   "    // Summons a spectral M16 for temporary use\n" +
                   "    float activeDuration = 5f; // The duration the M16 is active\n" +
                   "    GameObject spectralM16 = Instantiate(particleSystem, transform.position + transform.forward, Quaternion.identity);\n" +
                   "    spectralM16.GetComponent<ParticleSystem>().startColor = Color.gray; // Visual effect to represent the spectral nature\n" +
                   "    spectralM16.AddComponent<DestroyAfterTime>().lifetime = activeDuration;\n" +
                   "    // Simulate firing mechanism\n" +
                   "    StartCoroutine(FireBullets(activeDuration));\n" +
                   "}\n" +
                   "\n" +
                   "IEnumerator FireBullets(float duration)\n" +
                   "{\n" +
                   "    float endTime = Time.time + duration;\n" +
                   "    while (Time.time < endTime)\n" +
                   "    {\n" +
                   "        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cylinder);\n" +
                   "        bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f); // Size of the bullet\n" +
                   "        bullet.transform.position = transform.position + transform.forward * 1.5f; // Starting position of the bullet\n" +
                   "        Rigidbody rb = bullet.AddComponent<Rigidbody>();\n" +
                   "        rb.useGravity = false;\n" +
                   "        rb.velocity = transform.forward * 50; // Speed of the bullet\n" +
                   "        var damageComponent = bullet.AddComponent<DamageOnCollision>();\n" +
                   "        damageComponent.damage = 20; // Damage caused by the bullet\n" +
                   "        Destroy(bullet, 2f); // Bullet gets destroyed after 2 seconds to clean up\n" +
                   "        yield return new WaitForSeconds(0.1f); // Fire rate of the M16\n" +
                   "    }\n" +
                   "}\n"
        };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }

    public override void Action()
    {
        // Summons a spectral M16 for temporary use
        float activeDuration = 5f; // The duration the M16 is active
        GameObject spectralM16 = Instantiate(particleSys, transform.position + transform.forward, Quaternion.identity);
        spectralM16.GetComponent<ParticleSystem>().startColor = Color.gray; // Visual effect to represent the spectral nature
        spectralM16.AddComponent<DestroyAfterTime>().lifetime = activeDuration;
        // Simulate firing mechanism
        StartCoroutine(FireBullets(activeDuration));
    }

    IEnumerator FireBullets(float duration)
    {
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.5f); // Size of the bullet
            bullet.transform.position = transform.position + transform.forward * 1.5f; // Starting position of the bullet
            Rigidbody rb = bullet.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = transform.forward * 50; // Speed of the bullet
            var damageComponent = bullet.AddComponent<DamageOnCollision>();
            damageComponent.damage = 20; // Damage caused by the bullet
            Destroy(bullet, 2f); // Bullet gets destroyed after 2 seconds to clean up
            yield return new WaitForSeconds(0.1f); // Fire rate of the M16
        }
    }
}
