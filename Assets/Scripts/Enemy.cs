using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [Header("Basic Information")]
    public string enemyName;
    public string enemyType;
    public string enemyDesc;

    [Header("Basic Attributes")]
    public float maxHP = 10f;
    public float currentHP;
    public float damageReduction = 0f;
    public float defense = 0f;
    public float damage = 1f;
    public float attackSpeed = 1f;
    public float speed = 1f;
    public bool flying = false; 
    public bool energyImmune = false;

    public float trackProgress = 0f; // Potentially temporary, stores how far along the path the enemy is for targeting

    public List<Effect> activeEffects;


    public void Start()
    {
        currentHP = maxHP;

        if (currentHP <=0)
        {
            Destroy(gameObject);
        }
    }


    public void Update()
    {
        trackProgress += speed * Time.deltaTime;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        foreach (Effect effect in activeEffects)
        {
            effect.ApplyEffect(this);

            effect.duration -= Time.deltaTime;
            if (effect.duration <= 0)
            {
                activeEffects.Remove(effect);
            }
        }
    }


    public void TakeDamage(float amount, Tower.DamageType damageType)
    {
        if (energyImmune && damageType == Tower.DamageType.Energy)
        {
            return;
        }

        if (damageType == Tower.DamageType.Physical)
        {
            amount -= defense;
        }
        
        if (damageType != Tower.DamageType.True)
        {
            amount -= amount * (damageReduction/100);  
        }

        if (amount <= 0)
        {
            currentHP -= 1;
        }
        else {
            currentHP -= amount;
        }
        
        Debug.Log("Damage taken: " + amount + "Current HP: " + currentHP);

        if (currentHP <=0)
        {
            Destroy(gameObject);
        }
    }

    
    public void AddEffect(Effect effect)
    {
        activeEffects.Add(effect);
    }
}
