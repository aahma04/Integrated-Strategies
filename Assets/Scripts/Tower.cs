using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Tower : MonoBehaviour
{
    [Header("Basic Information")]
    public string towerName;
    public string towerType;
    public string desc;

    [Header("Basic Attributes")]
    public float maxHP = 10f;
    public float damageReduction = 0f; //Percentage
    public float defense = 0f; //Flat
    public float damage = 1f;
    public float attackSpeed = 1f;
    public float range = 0f;
    public float attackCooldown = 0f; //works as an original spawn delay + helps incorporate atkSpd

    [Header("Attack Info")]
    public DamageType damageType;
    public ProjectileType projectileType;
    public float skillCooldown = 1f;
    public float skillDmg = 10f;


    [Header("Type")]
    public string attackType; //"Single" , "AOE", etc. might be changed later

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

    private CircleCollider2D attackArea;

    protected List<Enemy> enemiesInRange;
    protected TargetPriority targetPriority = TargetPriority.First;

    protected bool specialUnlocked = false;

    protected List<Effect> activeEffects;


    private void Start()
    {
        enemiesInRange = new List<Enemy>();
        attackArea = GetComponent<CircleCollider2D>();

        attackArea.radius = range;

        activeEffects = new List<Effect>();
    }


    private void Update()
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


    private Enemy GetTarget()
    {
        switch (targetPriority)
        {
            case TargetPriority.First:
                return enemiesInRange.OrderByDescending(e => e.trackProgress).FirstOrDefault();
            case TargetPriority.Last:
                return enemiesInRange.OrderBy(e => e.trackProgress).FirstOrDefault();
            case TargetPriority.Close:
                return enemiesInRange.OrderBy(e => Vector2.Distance(transform.position, e.transform.position)).FirstOrDefault();
            case TargetPriority.Strong:
                return enemiesInRange.OrderByDescending(e => e.maxHP).FirstOrDefault();
        }

        return null;
    }


    private void Attack(Enemy target)
    {
        target.TakeDamage(damage, damageType);
    }


    public void UnlockSpecial()
    {
        specialUnlocked = true;
    }


    public void AddEffect(Effect effect)
    {
        activeEffects.Add(effect);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemiesInRange.Add(enemy);
            }
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }
}
