using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Basic Information")]
    public string enemyName;
    public string enemyType;
    public string enemyDesc;

    [Header("Basic Attributes")]
    public float maxHP = 10f;
    public float currentHP;
    public float dmgRed = 0f;
    public float defense = 0f;
    public float damage = 1f;
    public float attackSpd = 1f;
    public float spd = 1f;

    [Header("Attack Info")]
    public float skillCooldown = 1f;
    public float skillDmg = 1f;
    public bool flying = false; 

    public void Start()
    {
        currentHP = maxHP;

        if (currentHP <=0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        amount -= defense;
        amount -= amount * (dmgRed/100);  
        
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
}
