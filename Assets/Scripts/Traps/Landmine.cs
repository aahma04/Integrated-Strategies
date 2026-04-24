using UnityEngine;

public class Landmine : Trap
{
    protected override void OnStep(Enemy target)
    {
        foreach (Enemy enemy in attackRange.enemiesInRange)
        {
            target.TakeDamage(damage, damageType, null);
        }

        Destroy(gameObject);
    }
}

