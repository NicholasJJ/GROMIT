using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackTimer
{
    public float attackDelay = 1;
    private float attackTime;

    public bool CanAttack()
    {
        return Time.time > attackTime + attackDelay;
    }

    public void Attack()
    {
        attackTime = Time.time;
    }
}

public class MeleeEnemy : RandomWalkEnemy
{
    bool attacking = false;
    [SerializeField] float visionRange = 2;
    [SerializeField] AttackTimer attackTimer;
    [SerializeField] float attackDamage = 10;
    [SerializeField] float attackRange = 1;

    protected override void WhileAlive()
    {
        if (attacking)
        {
            AttackUpdate();
        }
        else
        {
            WanderUpdate();
            CheckForPlayer();
        }
    }

    void AttackUpdate()
    {
        if (!NearPlayer(attackRange))
        {
            MoveToPlayer();
        }
        if (NearPlayer(attackRange) && attackTimer.CanAttack())
        {
            attackTimer.Attack();
            animator.SetTrigger("Attack");
            var health = player.GetComponent<FitzHealth>();
            if (health)
            {
                health.CauseDamage(attackDamage, gameObject);
            }
        }
    }

    void CheckForPlayer()
    {
        bool inView = Vector3.Dot((player.transform.position - transform.position).normalized, transform.forward) > 0;
        bool inRange = Vector3.Distance(player.transform.position, transform.position) < visionRange;
        attacking = inView && inRange;
    }
}
