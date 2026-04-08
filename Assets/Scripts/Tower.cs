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
    public float dmgRed = 0f; //Percentage
    public float defense = 0f; //Flat
    public float damage = 1f;
    public float atkSpd = 1f;
    public float range = 0f;
    public float attackCooldown = 0f; //works as an original spawn delay + helps incorporate atkSpd

    [Header("Attack Info")]
    public string dmgType;
    public string projType;
    public bool highTile = true;
    public bool lowTile = true;
    public float skillCooldown = 1f;
    public float skillDmg = 10f;


    [Header("Type")]
    public string attackType; //"Single" , "AOE", etc. might be changed later

    private enum TargetPriority
    {
        First,
        Last,
        Close,
        Strong
    }

    private CircleCollider2D attackArea;

    private List<Enemy> enemiesInRange;
    private TargetPriority targetPriority = TargetPriority.First;


    private void Start()
    {
        enemiesInRange = new List<Enemy>();
        attackArea = GetComponent<CircleCollider2D>();

        attackArea.radius = range;
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
            attackCooldown = 1f / atkSpd;
        }
    }


    // OLD
    // private Enemy FindFirstEnemyInRange()
    // {
    //     Enemy[] enemies = FindObjectsOfType<Enemy>();

    //     foreach (Enemy enemy in enemies)
    //     {
    //         if (enemy == null)
    //         continue;
    //         float distance = Vector2.Distance(transform.position, enemy.transform.position);
    //         if (distance <= range)
    //             return enemy;
    //     }
    //     return null;
    // }


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
        target.TakeDamage(damage);
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
