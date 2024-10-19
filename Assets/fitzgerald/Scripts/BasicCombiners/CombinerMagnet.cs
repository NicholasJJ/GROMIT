using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerMagnet : CombinerObject
{

    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "Magnet",
            display = "1F9F2",
            code = "public override void Action()\n" +
                "{\n" +
                "   float explosionRadius = 20;\n" +
                    "   float explosionForce = 200;\n" +
                    "   // Get all colliders within the explosion radius\n" +
                    "   Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);\n" +
                    "   \n" +
                    "   foreach (Collider hit in colliders)\n" +
                    "   {\n" +
                    "       // Check if the object has a Rigidbody component\n" +
                    "       Rigidbody rb = hit.GetComponent<Rigidbody>();\n" +
                    "       if (rb != null)\n" +
                    "       {\n" +
                    "           // Apply explosion force to the Rigidbody\n" +
                    "           rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);\n" +
                    "       }\n" +
                    "   }" +
                "}\n"
                
    };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }

    public override void Action()
    {
        float explosionRadius = 20;
        float explosionForce = 200;
        // Get all colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            // Check if the object has a Rigidbody component
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply explosion force to the Rigidbody
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }
}
