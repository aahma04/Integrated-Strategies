using UnityEngine;
using System.Collections.Generic;

public class Splash : Tower
{
    [Header("Path 3")]
    public float shredAmount;
    public float shredDuration;

    private GameObject attackNode;
    private PolygonCollider2D attackCollider;

    private List<Collider2D> collidersInDamageArea;


    protected override void Start()
    {
        base.Start();
        attackNode = transform.Find("AttackNode").gameObject;
        attackCollider = attackNode.GetComponent<PolygonCollider2D>();

        collidersInDamageArea = new List<Collider2D>();
    }


    protected override void Attack(Enemy target)
    {
        if (specialUnlocked == 1)
        {
            foreach (Enemy enemy in attackRange.enemiesInRange)
            {
                enemy.TakeDamage(damage, damageType, this);
            }
        }
        else
        {
            attackNode.transform.right = target.transform.position - transform.position;

            foreach (GameObject enemyObj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (attackCollider.bounds.Intersects(enemy.GetComponent<Collider2D>().bounds))
                {
                    enemy.TakeDamage(damage, damageType, this);

                    if (specialUnlocked == 2)
                    {
                        enemy.ApplyShred(shredAmount, shredDuration);
                    }
                }
            }
        }
    }
}
