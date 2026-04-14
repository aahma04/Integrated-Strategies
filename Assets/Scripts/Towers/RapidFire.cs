using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RapidFire : Tower
{
    private Enemy[] GetTarget()
    {
        int numTargets = (specialUnlocked == 1) ? 2 : 1;

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
        foreach (Enemy target in targets)
        {
            target.TakeDamage(damage, damageType, this);
        }
    }
}
