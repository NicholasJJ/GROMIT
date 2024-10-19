using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TakeDamageEvent : UnityEvent<float, GameObject> { }

[System.Serializable]
public class DeathEvent : UnityEvent<GameObject> { }

public class FitzHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float damageScale = 1.0f;

    public TakeDamageEvent OnTakeDamage;
    public DeathEvent OnDeath;

    public float recoveryTime = 0;
    float lastDamageTime;

    public void Start()
    {
        currentHealth = maxHealth;
        var overheadUIs = GetComponentsInChildren<OverheadUI>();
        foreach (var ui in overheadUIs)
        {
            OnTakeDamage.AddListener(ui.AddDamageText);
        }
    }

    public void CauseDamage(float damage, GameObject causer)
    {
        if (Time.time <= lastDamageTime + recoveryTime) return;
        Debug.Log("Got Hurt!");
        lastDamageTime = Time.time;
        float effectiveDamage = damage * damageScale;
        currentHealth -= effectiveDamage;

        OnTakeDamage.Invoke(effectiveDamage, causer);

        if (currentHealth <= 0)
        {
            OnDeath.Invoke(causer);
        }
    }

    public float GetHealthPct()
    {
        return currentHealth / maxHealth;
    }
}
