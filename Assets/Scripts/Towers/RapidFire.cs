using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RapidFire : Tower
{   
    // Path 1 will upgrade numTargets from Tower class
    private List<SpriteRenderer> attackEffects;

    [Header("Path 2")]
    public float criticalChance;
    public float criticalMultiplier;

    [Header("Path 3")]
    public float adrenalineMultiplier;


    protected override void Awake()
    {
        base.Awake();

        attackEffects = new List<SpriteRenderer>();
        attackEffects.Add(attackEffect);
    }


    public override void BuySpecial(int pathIndex)
    {
        base.BuySpecial(pathIndex);

        if (specialUnlocked == 1)
        {
            numTargets = 2;

            attackEffects.Add(transform.Find("AttackEffect1").GetComponent<SpriteRenderer>());

            attackEffects[numTargets-1].enabled = false;
        }
    }


    protected override void Attack(Enemy[] targets)
    {
        float attackDamage = damage;

        if (specialUnlocked == 2 && Random.value < criticalChance)
        {
            attackDamage *= criticalMultiplier;
        }

        if (specialUnlocked == 3)
        {
            attackDamage *= ((targets[0].trackProgress / (targets[0].path.Count) * adrenalineMultiplier)+1);
        }

        for (int i=0; i < targets.Length; i++)
        {
            targets[i].TakeDamage(attackDamage, damageType, this);
            StartCoroutine(DoAttackEffect(attackEffects[i], targets[i]));
        }
    }
}
