using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RapidFire : Tower
{   
    [Header("Path 1")]
    public int numTargets = 1;

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

    private Enemy[] GetTarget()
    {

        if (attackRange.enemiesInRange.Count == 0)
        {
            return null;
        }
        else if (attackRange.enemiesInRange.Count < numTargets)
        {
            numTargets = attackRange.enemiesInRange.Count;
        }

        switch (targetPriority)
        {
            case TargetPriority.First:
                return attackRange.enemiesInRange.OrderByDescending(e => e.trackProgress).Take(numTargets).ToArray();
            case TargetPriority.Last:
                return attackRange.enemiesInRange.OrderBy(e => e.trackProgress).Take(numTargets).ToArray();
            case TargetPriority.Close:
                return attackRange.enemiesInRange.OrderBy(e => Vector2.Distance(transform.position, e.transform.position)).Take(numTargets).ToArray();
            case TargetPriority.Strong:
                return attackRange.enemiesInRange.OrderByDescending(e => e.maxHP).Take(numTargets).ToArray();
        }

        return null;
    }


    private void Attack(Enemy[] targets)
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
