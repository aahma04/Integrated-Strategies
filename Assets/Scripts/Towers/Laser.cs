using UnityEngine;
using System.Collections.Generic;

public class Laser : Tower
{
    [Header("Path 1")]
    public float damageRampRate = 0.5f;
    public float damageAmpCap = 2.5f;
    
    [Header("Path 2")]
    public float special2SlowAmount = 0.5f;

    [Header("Path 3")]
    public float chainDamageFalloff;
    public float chainRange;
    public int numBounces;

    private float damageAmp = 1f;

    private Enemy currentTarget;

    private GameObject attackNode;
    private AttackRange attackNodeScript;


    protected override void Awake()
    {
        base.Awake();
        attackNode = transform.Find("AttackNode").gameObject;
        attackNodeScript = attackNode.GetComponent<AttackRange>();
        attackNodeScript.SetRange(chainRange);
    }


    protected override void Attack(Enemy target)
    {
        if (currentTarget != target)
        {
            damageAmp = 1f;
            currentTarget = target;
        }

        target.TakeDamage(damage * damageAmp, damageType, this);

        if (specialUnlocked == 1)
        {
            damageAmp = Mathf.Min(damageAmp + (Time.deltaTime * damageRampRate), damageAmpCap);
        }

        if (specialUnlocked == 2)
        {
            target.ApplySlow(special2SlowAmount, 1/attackSpeed);
        }

        if (specialUnlocked == 3)
        {
            List<Enemy> bouncedEnemies = new List<Enemy>();
            Enemy bounceTarget = currentTarget;
            float bounceDamage = damage;

            for (int i=0; i<numBounces; i++)
            {
                attackNode.transform.position = bounceTarget.transform.position;
                Enemy enemyToHit = attackNodeScript.GetTarget(1, bounceTarget.transform, TargetPriority.Close)[0];
                bounceDamage *= chainDamageFalloff;
                enemyToHit.TakeDamage(bounceDamage, damageType, this);
                bounceTarget = enemyToHit;
            }
        }
    }
}
