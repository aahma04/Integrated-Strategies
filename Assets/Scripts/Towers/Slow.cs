using UnityEngine;
// using System.Collections;

public class Slow : Tower
{
    [Header("Path 1")]
    public float slowRadius = 1.5f;

    [Header("Path 2")]
    public float slowAmount = 0.4f;

    [Header("Path 3")]
    public float damageAmp;
    public float slowDuration = 5f;

    private GameObject attackNode;
    private CircleCollider2D attackCollider;
    private AttackRange attackNodeScript;

    private Enemy currentTarget;


    protected override void Start()
    {
        base.Start();
        attackNode = transform.Find("AttackNode").gameObject;
        attackCollider = attackNode.GetComponent<CircleCollider2D>();
        attackCollider.radius = slowRadius;
        attackNodeScript = attackNode.GetComponent<AttackRange>();
    }


    public override void BuySpecial(int pathIndex)
    {
        base.BuySpecial(pathIndex);

        if (specialUnlocked == 2)
        {
            slowAmount = 0.7f;
        }
    }


    protected override void Attack(Enemy target)
    {
        if (specialUnlocked == 1)
        {
            attackNode.transform.position = target.transform.position;

            Enemy[] enemiesToHit = attackNodeScript.enemiesInRange.ToArray();
            Debug.Log("Enemies hit by slow: " + enemiesToHit.Length);

            foreach (Enemy enemy in enemiesToHit)
            {
                ApplyAttack(enemy);
            }
        }
        else
        {
            ApplyAttack(target);
        }
    }

    private void ApplyAttack(Enemy target)
    {
        target.TakeDamage(damage, damageType, this);

        target.ApplySlow(slowAmount, slowDuration);
        
        if (specialUnlocked == 3)
        {
            target.ApplyWeakness(damageAmp, slowDuration);
        }
    }

}
