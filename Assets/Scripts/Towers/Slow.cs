using UnityEngine;
// using System.Collections;

public class Slow : Tower
{
    public float slowDuration = 5f;
    public float slowAmount = 0.6f;
    public float slowRadius = 1.5f;

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

        specialUnlocked = true; // for testing
        attackNodeScript = attackNode.GetComponent<AttackRange>();
    }


    protected override void Attack(Enemy target)
    {
        if (specialUnlocked)
        {
            attackNode.transform.position = target.transform.position;

            Enemy[] enemiesToHit = attackNodeScript.enemiesInRange.ToArray();
            Debug.Log("Enemies hit by slow: " + enemiesToHit.Length);

            foreach (Enemy enemy in enemiesToHit)
            {
                ApplySlow(enemy);
            }
        }
        else
        {
            ApplySlow(target);
        }
    }

    private void ApplySlow(Enemy target)
    {
        target.TakeDamage(damage, damageType, this);

        target.speedModifier = Mathf.Min(target.speedModifier, 1-slowAmount);
        target.speedModifierDuration = Mathf.Max(target.speedModifierDuration, slowDuration);
    }

}
