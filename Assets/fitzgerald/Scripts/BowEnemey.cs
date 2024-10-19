using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowEnemey : FitzEnemy
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float wanderSpeed;
    [SerializeField] float approachSpeed;
    bool shooting = true;
    [SerializeField] float shootingTime;
    [SerializeField] float reloadTime;
    float lastShootTime = 0;
    float shootStartTime = 0;

    protected override void WhileAlive()
    {
        // Debug.Log("!!!");
        if (shooting)
        {
            ShootUpdate();
        }
        else
        {
            WanderUpdate();
        }
    }

    void ShootUpdate()
    {
        MoveToPlayer();
        agent.speed = approachSpeed;
        //Debug.Log("Shooting");
        if (Time.time > shootStartTime + shootingTime)
        {
            MoveToRandomPoint();
            agent.speed = wanderSpeed;
            shooting = false;
        }
        else
        {
            if (Time.time > lastShootTime + reloadTime)
            {
                lastShootTime = Time.time;
                Shoot();
            }
        }
    }

    void WanderUpdate()
    {
        //Debug.Log($"Wandering {agent.remainingDistance} {agent.destination}");
        if (agent.remainingDistance <= 0.001f)
        {
            MoveToPlayer();
            shootStartTime = Time.time;
            agent.speed = approachSpeed;
            shooting = true;
        }
    }

    void Shoot()
    {
        GameObject arrow = Instantiate(arrowPrefab);
        arrow.transform.position = transform.position + (player.transform.position - transform.position).normalized * 1.5f;
        arrow.transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
        arrow.GetComponent<Rigidbody>().velocity = arrow.transform.forward * 4;
    }
}
