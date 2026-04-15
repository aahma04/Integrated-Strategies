using UnityEngine;
using System.Collections;

public class Flamethrower : Tower
{
    [Header("General")]
    public float burnDuration;

    [Header("Path 1")]
    public int burnStackLimit = 1;

    [Header("Path 2")]
    public float mainTargetDamage;

    [Header("Path 3")]
    public float lavaPoolDamage;
    public float lavaPoolDuration;

    private GameObject attackNode;
    private BoxCollider2D attackCollider;
    private AttackRange attackNodeScript;


    protected override void Start()
    {
        base.Start();
        attackNode = transform.Find("AttackNode").gameObject;
        attackCollider = attackNode.GetComponent<BoxCollider2D>();

        attackNodeScript = attackNode.GetComponent<AttackRange>();
    }


    protected override void Attack(Enemy target)
    {
        attackNode.transform.right = target.transform.position - transform.position;

        Enemy[] enemiesToHit = attackNodeScript.enemiesInRange.ToArray();

        Debug.Log("Enemies hit by flamethrower: " + enemiesToHit.Length);
        foreach (Enemy enemy in enemiesToHit)
        {
            ApplyAttack(enemy);
        }

        if (specialUnlocked == 2)
        {
            target.TakeDamage(mainTargetDamage, DamageType.Physical, this);
        }
        else if (specialUnlocked == 3)
        {
            // Do lava pools later
        }
    }


    private void ApplyAttack(Enemy target)
    {
        Debug.Log("Checking burn for " + target.name);
        // Check if target is either not burning or if stacking special unlocked
        if (CountBurnStacks(target) < burnStackLimit)
        {
            target.AddEffect(new Burning(damage, burnDuration, this));
            Debug.Log("Applied burn to " + target.name);
        }
    }


    private int CountBurnStacks(Enemy target)
    {
        int count = 0;
        foreach (Effect effect in target.activeEffects)
        {
            if (effect is Burning)
            {
                count++;
            }
        }
        return count;
    }
}
