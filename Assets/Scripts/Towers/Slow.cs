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

    [Header("Perk Version")]
    public bool isAttackMode;
    public float attackModeDamage = 100f;
    public float attackModeAttackSpeed = 1f;
    public float attackModeSlowAmount = 0.15f;

    private GameObject attackNode;
    private AttackRange attackNodeScript;


    protected override void Awake()
    {
        base.Awake();
        attackNode = transform.Find("AttackNode").gameObject;
        attackNodeScript = attackNode.GetComponent<AttackRange>();
        attackNodeScript.SetRange(slowRadius);
    }


    public void EnterAttackMode()
    {
        damage = attackModeDamage;
        attackSpeed = attackModeAttackSpeed;
        slowAmount = attackModeSlowAmount;
    }


    public override void BuySpecial(int pathIndex)
    {
        base.BuySpecial(pathIndex);

        if (specialUnlocked == 2)
        {
            slowAmount += 0.3f;
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
