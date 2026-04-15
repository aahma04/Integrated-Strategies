using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RapidFire : Tower
{   
    // Path 1 will upgrade numTargets from Tower class

    [Header("Path 2")]
    public float criticalChance;
    public float criticalMultiplier;

    [Header("Path 3")]
    public float adrenalineMultiplier;


    public override void BuySpecial(int pathIndex)
    {
        base.BuySpecial(pathIndex);

        if (specialUnlocked == 1)
        {
            numTargets = 2;
        }
    }


    protected void MultiAttack(Enemy[] targets)
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

        foreach (Enemy target in targets)
        {
            target.TakeDamage(attackDamage, damageType, this);
        }
    }
}
