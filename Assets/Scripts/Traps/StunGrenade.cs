using UnityEngine;

public class StunGrenade : Trap
{
    public float stunDuration;

    protected override void TimeOut()
    {
        foreach (Enemy enemy in attackRange.enemiesInRange)
        {
            enemy.TakeDamage(damage, damageType, null);
            enemy.ApplySlow(1f, stunDuration);
        }
    }
}
