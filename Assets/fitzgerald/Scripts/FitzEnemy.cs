using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FitzEnemy : MonoBehaviour
{
    protected FitzHealth health;
    protected FitzRoomVolume roomVol;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected GameObject player;
    public List<GameObject> deathSpawns = new List<GameObject>();

    private float playerSize = 0.5f;

    //[SerializeField] 

    void Start()
    {
        Debug.Log("Start");
        animator = GetComponentInChildren<Animator>();
        var collider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();

        health = GetComponent<FitzHealth>();
        if (health)
        {
            health.OnDeath.AddListener((killer) =>
            {
                // Destroy(gameObject);
                animator.SetTrigger("Death");
                collider.enabled = false;
                agent.isStopped = true;
                agent.enabled = false;
                if (deathSpawns.Count == 0) return;
                Instantiate(deathSpawns[Random.Range(0, deathSpawns.Count)]).transform.position = transform.position;
            });
        }
        player = GameObject.FindGameObjectWithTag("Player");
        var playerController = player.GetComponent<CharacterController>();
        if (playerController)
        {
            playerSize = playerController.radius;
        }
    }

    public void OnSpawn(EnemySpawner spawner)
    {
        Debug.Log($"! {spawner.rooms[0]}");
        roomVol = spawner.rooms[0];
    }
    // private void OnTriggerEnter(Collider other) {
    //     var vol = other.gameObject.GetComponent<FitzRoomVolume>();
    //     if (vol) {
    //         roomVol = vol;
    //     }
    // }

    //void Update()
    //{
    //    Debug.Log($"UPDATE {health.currentHealth}, {agent}, {roomVol}");
    //    if (health.currentHealth > 0 && agent != null && roomVol != null)
    //    {
    //        if (agent.remainingDistance <= 0.001f)
    //        {
    //            var newDest = roomVol.RandomPointInRoom();
    //            newDest.y = transform.position.y;

    //            //Debug.Log("Moving to " + newDest);

    //            agent.destination = newDest;
    //        }

    //        animator.SetFloat("Speed", agent.velocity.magnitude);
    //    }
    //}

    void Update()
    {
        if (health.currentHealth > 0 && agent && roomVol)
        {
            WhileAlive();
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    protected virtual void WhileAlive()
    {

    }

    protected void MoveToPlayer()
    {
        var newDest = player.transform.position;
        newDest.y = transform.position.y;
        agent.destination = newDest;
    }

    protected void MoveToRandomPoint()
    {
        var newDest = roomVol.RandomPointInRoom();
        newDest.y = transform.position.y;
        agent.destination = newDest;
    }

    protected bool NearPlayer(float distance)
    {
        return (player.transform.position - transform.position).magnitude <= (agent.radius + playerSize + distance);
    }
}
