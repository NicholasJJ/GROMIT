using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    [SerializeField] public float damage;
    public bool damageEnemies = true;
    public bool damagePlayer = false;
    private void OnCollisionEnter(Collision collision)
    {
        var health = collision.gameObject.GetComponent<FitzHealth>();
        if (health)
        {
            CauseHealthDamage(health);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var health = other.gameObject.GetComponent<FitzHealth>();
        if (health)
        {
            if ((other.gameObject.GetComponent<FitzPlayer>() && damagePlayer) ||
                (other.gameObject.GetComponent<FitzEnemy>() && damageEnemies) ||
                (!other.gameObject.GetComponent<FitzEnemy>() && !other.gameObject.GetComponent<FitzPlayer>()))
                    CauseHealthDamage(health);
        }
    }

    public void CauseHealthDamage(FitzHealth damageTaker) {
        if (damageTaker) {
            damageTaker.CauseDamage(damage, gameObject);
        }
    }
}
