using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinerFire : CombinerObject
{
    // Start is called before the first frame update
    void Start()
    {
        CombinerObjectData data = new CombinerObjectData
        {
            description = "Fire",
            display = "1F525",
            code = "public override void Action()\n" +
                    "{\n" +
                    "    // Spawns a stationary fireball in front of the player for a few seconds\n" +
                    "    GameObject fireball = GameObject.CreatePrimitive(PrimitiveType.Sphere);\n" +
                    "    fireball.GetComponent<Renderer>().material.color = Color.red;\n" +
                    "    fireball.transform.position = transform.position + transform.forward * 2;\n" +
                    "    fireball.GetComponent<Collider>().isTrigger = true;\n" +
                    "    fireball.AddComponent<DamageOnCollision>().damage = 30;\n" +
                    "    // Add a fire effect\n" +
                    "    GameObject fireParticles = Instantiate(particleSys);\n" +
                    "    fireParticles.transform.position = fireball.transform.position;\n" +
                    "    fireParticles.transform.parent = fireball.transform;\n" +
                    "    var main = fireParticles.GetComponent<ParticleSystem>().main;\n" +
                    "    main.startColor = Color.red;\n" +
                    "    // Destroy the fireball after 2 seconds\n" +
                    "    fireball.AddComponent<DestroyAfterTime>().lifetime = 2;\n" +
                    "}"
    };
        Setup(data);
        CombinerManager.Instance.AddExample(data);
    }

    public override void Action()
    {
        //Spawns a stationary fireball in front of the player for a few seconds
        GameObject fireball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fireball.GetComponent<Renderer>().material.color = Color.red;
        fireball.transform.position = transform.position + transform.forward * 2;
        fireball.GetComponent<Collider>().isTrigger = true;
        fireball.AddComponent<DamageOnCollision>().damage = 30;
        //Add a fire effect
        GameObject fireParticles = Instantiate(particleSys);
        fireParticles.transform.position = fireball.transform.position;
        fireParticles.transform.parent = fireball.transform;
        var main = fireParticles.GetComponent<ParticleSystem>().main;
        main.startColor = Color.red;
        //destroy the firball after 2 seconds
        fireball.AddComponent<DestroyAfterTime>().lifetime = 1;
    }

    
}
