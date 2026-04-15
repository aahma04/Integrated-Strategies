using UnityEngine;

public class Scout : Tower
{
    [Header("General")]
    public float silenceDuration = 3f;

    [Header("Path 1")]
    public float concussDamageAmp = 1f;

    public override void BuySpecial(int pathIndex)
    {
        base.BuySpecial(pathIndex);

        if (specialUnlocked == 1)
        {
            concussDamageAmp = 3;
        }
        else if (specialUnlocked == 2)
        {
            UpdateRange(100f);
        }
        else if (specialUnlocked == 3)
        {
            UpdateRange(4f);
        }
    }

    protected override void Attack(Enemy target)
    {
        if (specialUnlocked == 3)
        {
            foreach (Enemy enemy in attackRange.enemiesInRange)
            {
                enemy.ApplySilence(silenceDuration);
            }
        }
        else
        {
            float actualDamage = damage;

            if (specialUnlocked == 1 && target.silenceDuration > 0)
            {
                actualDamage *= concussDamageAmp;
            }

            target.TakeDamage(damage, damageType, this);

            target.ApplySilence(silenceDuration);
        }
    }
}
