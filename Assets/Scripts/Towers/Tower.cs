using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct Upgrade
{
    public int cost;
    public float effectAmount;
}

[System.Serializable]
public struct SpecialUpgrade
{
    public string name;
    public string description;
    public int cost;
    public string newTowerName;
}

public class Tower : MonoBehaviour
{
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

    [Header("Basic Information")]
    public string towerName;
    public string towerType;
    public string description;
    public int cost;
    public Color towerColor;

    [Header("Basic Attributes")]
    // public float maxHP = 10f;
    // public float damageReduction = 0f; //Percentage
    // public float defense = 0f; //Flat
    public float damage = 1f;
    public float attackSpeed = 1f;
    public float range = 0f;
    protected float attackCooldown = 0f; //works as an original spawn delay + helps incorporate atkSpd
    public int numTargets = 1;

    [Header("Attack Info")]
    public DamageType damageType;
    public ProjectileType projectileType;
    // public float skillCooldown = 1f;
    // public float skillDmg = 10f;


    // [Header("Type")]
    // public string attackType; //"Single" , "AOE", etc. might be changed later

    [Header("Upgrade Info")]
    public Upgrade[] damageUpgrades;
    public Upgrade[] attackSpeedUpgrades;
    public Upgrade[] rangeUpgrades;

    public SpecialUpgrade[] specialUpgrades;

    [HideInInspector]
    public int damageUpgradeLevel = 0;
    [HideInInspector]
    public int attackSpeedUpgradeLevel = 0;
    [HideInInspector]
    public int rangeUpgradeLevel = 0;
    [HideInInspector]
    public int specialUnlocked = 0;
    [HideInInspector]
    public AttackRange attackRange; // needs to be public so enemies can remove themselves from tower range when they die
    [HideInInspector]
    public TargetPriority targetPriority = TargetPriority.First;

    protected List<Effect> activeEffects;


    protected virtual void Awake()
    {
        attackRange = GetComponentInChildren<AttackRange>();
        activeEffects = new List<Effect>();
    }


    protected virtual void Update()
    {
        foreach (Effect effect in activeEffects)
        {
            effect.ApplyEffect(this);

            effect.duration -= Time.deltaTime;
            if (effect.duration <= 0)
            {
                activeEffects.Remove(effect);
            }
        }

        attackCooldown -= Time.deltaTime;
        if (attackCooldown > 0f)
        {
            return;
        }

        attackRange.SetRange(range);
        Enemy[] targets = attackRange.GetTarget(numTargets, transform, targetPriority);
        if (targets != null)
        {
            if (targets.Length > 1)
            {
                MultiAttack(targets);
            }
            else
            {
                Attack(targets[0]);
            }
            attackCooldown = 1f / attackSpeed;
        }
    }


    protected virtual void Attack(Enemy target)
    {
        target.TakeDamage(damage, damageType, this);
    }


    protected virtual void MultiAttack(Enemy[] targets)
    {
        return;
    }


    public void AddEffect(Effect effect)
    {
        activeEffects.Add(effect);
    }


    public virtual void BuySpecial(int pathIndex)
    {
        if (specialUnlocked != 0)
            return;

        specialUnlocked = pathIndex + 1;
        towerName = specialUpgrades[pathIndex].newTowerName;
        description += $"\n{specialUpgrades[pathIndex].description}";
    }


    public void ChangePriority()
    {
        targetPriority = (TargetPriority)(((int)targetPriority + 1) % System.Enum.GetValues(typeof(TargetPriority)).Length);
    }


    public void UpdateRange(float newRange)
    {
        range = newRange;
        if (attackRange == null)
        {
            Debug.Log("null area");
            return;
        }
        attackRange.SetRange(newRange);
    }
}
