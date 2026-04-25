using UnityEngine;
using System.Collections.Generic;

public class Splash : Tower
{
    [Header("Path 2")]
    public float maxDamageAmp = 5f;
    private float damageAmp = 1f;

    [Header("Path 3")]
    public float shredAmount;
    public float shredDuration;

    private GameObject attackNode;
    private AttackRange attackNodeScript;


    protected override void Awake()
    {
        base.Awake();
        attackNode = transform.Find("AttackNode").gameObject;
        attackNodeScript = attackNode.GetComponent<AttackRange>();
    }


    protected override void Attack(Enemy[] targets)
    {
        Enemy target = targets[0];
        
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

            Enemy[] enemiesToHit = attackNodeScript.enemiesInRange.ToArray();

            foreach (Enemy enemy in enemiesToHit)
            {
                if (specialUnlocked == 2)
                {
                    damageAmp = maxDamageAmp/(Mathf.Sqrt(enemiesToHit.Length));
                }

                enemy.TakeDamage(damage*damageAmp, damageType, this);

                if (specialUnlocked == 3)
                {
                    enemy.ApplyShred(shredAmount, shredDuration);
                }
            }
            
            StartCoroutine(DoAttackEffect(attackEffect, target));
        }
    }
}
