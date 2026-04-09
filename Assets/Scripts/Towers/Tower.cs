using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Tower : MonoBehaviour
{
    [Header("Basic Information")]
    public string towerName;
    public string towerType;
    public string description;
    public int cost;

    [Header("Basic Attributes")]
    // public float maxHP = 10f;
    // public float damageReduction = 0f; //Percentage
    // public float defense = 0f; //Flat
    public float damage = 1f;
    public float attackSpeed = 1f;
    public float range = 0f;
    protected float attackCooldown = 0f; //works as an original spawn delay + helps incorporate atkSpd

    [Header("Attack Info")]
    public DamageType damageType;
    public ProjectileType projectileType;
    // public float skillCooldown = 1f;
    // public float skillDmg = 10f;


    // [Header("Type")]
    // public string attackType; //"Single" , "AOE", etc. might be changed later

    [Header("Upgrade Info")]
    public int[] damageUpgradeCosts;
    public int[] attackSpeedUpgradeCosts;
    public int[] rangeUpgradeCosts;

    public int damageUpgradeLevel = 0;
    public int attackSpeedUpgradeLevel = 0;
    public int rangeUpgradeLevel = 0;

    public int specialCost;
    public bool specialUnlocked = false;

    public enum TargetPriority
    {
        First,
        Last,
        Close,
        Strong
    }

    public enum DamageType
    {
        Physical,
        Energy,
        True
    }

    public enum ProjectileType
    {
        Single,
        Area
    }

    // public List<Enemy> enemiesInRange;
    public AttackRange attackRange;
    protected TargetPriority targetPriority = TargetPriority.First;


    protected List<Effect> activeEffects;


    protected virtual void Start()
    {
        attackRange = GameObject.Find("AttackRange").transform.GetComponent<AttackRange>();
        activeEffects = new List<Effect>();
    }


    protected virtual void Update()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown > 0f)
        {
            return;
        }

        Enemy target = GetTarget();
        if (target != null)
        {
            Attack(target);
            attackCooldown = 1f / attackSpeed;
        }

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


    protected virtual Enemy GetTarget()
    {
        if (attackRange.enemiesInRange.Count == 0)
        {
            return null;
        }

        switch (targetPriority)
        {
            case TargetPriority.First:
                return attackRange.enemiesInRange.OrderByDescending(e => e.trackProgress).FirstOrDefault();
            case TargetPriority.Last:
                return attackRange.enemiesInRange.OrderBy(e => e.trackProgress).FirstOrDefault();
            case TargetPriority.Close:
                return attackRange.enemiesInRange.OrderBy(e => Vector2.Distance(transform.position, e.transform.position)).FirstOrDefault();
            case TargetPriority.Strong:
                return attackRange.enemiesInRange.OrderByDescending(e => e.maxHP).FirstOrDefault();
        }

        return null;
    }


    protected virtual void Attack(Enemy target)
    {
        target.TakeDamage(damage, damageType, this);
    }


    public void UnlockSpecial()
    {
        specialUnlocked = true;
    }


    public void AddEffect(Effect effect)
    {
        activeEffects.Add(effect);
    }


    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Enemy"))
    //     {
    //         Enemy enemy = other.GetComponent<Enemy>();
    //         if (enemy != null)
    //         {
    //             enemiesInRange.Add(enemy);
    //         }
    //     }
    // }


    // void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Enemy"))
    //     {
    //         Enemy enemy = other.GetComponent<Enemy>();
    //         if (enemy != null)
    //         {
    //             enemiesInRange.Remove(enemy);
    //         }
    //     }
    // }
}
