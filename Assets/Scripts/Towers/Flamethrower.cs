using UnityEngine;
using System.Collections;

public class Flamethrower : Tower
{
    public float burnDuration;

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
            ApplyBurn(enemy);
        }
    }


    private void ApplyBurn(Enemy target)
    {
        Debug.Log("Checking burn for " + target.name);
        // Check if target is either not burning or if special unlocked (stacking enabled)
        if (!target.activeEffects.Exists(effect => effect is Burning) || (specialUnlocked == 1))
        {
            target.AddEffect(new Burning(damage, burnDuration, this));
            Debug.Log("Applied burn to " + target.name);
        }
    }
}
