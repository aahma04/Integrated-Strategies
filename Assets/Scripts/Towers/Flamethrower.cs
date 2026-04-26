using UnityEngine;
using System.Collections;

public class Flamethrower : Tower
{
    [Header("General")]
    public float burnDuration;
    private Vector3 attackEffectMinScale;
    private Vector3 attackEffectMaxScale;

    [Header("Path 1")]
    public int burnStackLimit = 1;

    [Header("Path 2")]
    public float mainTargetDamage;

    [Header("Path 3")]
    public float lavaPoolDamage;
    public float lavaPoolDuration;

    private GameObject attackNode;
    private AttackRange attackNodeScript;


    protected override void Awake()
    {
        base.Awake();
        attackNode = transform.Find("AttackNode").gameObject;
        attackNodeScript = attackNode.GetComponent<AttackRange>();

        attackEffectMaxScale = attackEffect.gameObject.transform.localScale;
        attackEffectMinScale = new Vector3(0f, attackEffectMaxScale.y, attackEffectMaxScale.z);
    }


    public override void BuySpecial(int pathIndex)
    {
        base.BuySpecial(pathIndex);

        if (specialUnlocked == 3)
        {
            attackNodeScript.checkTiles = true;
        }
    }


    protected override void Attack(Enemy[] targets)
    {
        Enemy target = targets[0];

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
            foreach (GameObject pathTile in attackNodeScript.tilesInRange)
            {
                continue;
            }
        }

        StartCoroutine(DoAttackEffect(attackEffect, target, 0.35f));
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


    public override IEnumerator DoAttackEffect(SpriteRenderer effectSprite, Enemy target, float duration=0.1f)
    {
        Transform effectObject = effectSprite.gameObject.transform;

        effectObject.right = target.transform.position - effectObject.position;
        effectSprite.enabled = true;

        float counter = 0f; 

        while (counter < (duration/2))
        {
            counter += Time.deltaTime;
            effectObject.localScale = Vector3.Lerp(attackEffectMinScale, attackEffectMaxScale, counter / (duration/2));
            yield return null;
        }
        yield return new WaitForSeconds(duration/2);
        effectSprite.enabled = false;
    }
}
