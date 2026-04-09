using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RapidFire : Tower
{
    private Enemy[] GetTarget()
    {
        int numTargets = specialUnlocked ? 2 : 1;

        switch (targetPriority)
        {
            case TargetPriority.First:
                return enemiesInRange.OrderByDescending(e => e.trackProgress).Take(numTargets).ToArray();
            case TargetPriority.Last:
                return enemiesInRange.OrderBy(e => e.trackProgress).Take(numTargets).ToArray();
            case TargetPriority.Close:
                return enemiesInRange.OrderBy(e => Vector2.Distance(transform.position, e.transform.position)).Take(numTargets).ToArray();
            case TargetPriority.Strong:
                return enemiesInRange.OrderByDescending(e => e.maxHP).Take(numTargets).ToArray();
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
