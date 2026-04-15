using UnityEngine;

public class Sniper : Tower
{
    // Path 1 will upgrade damage from tower class
    
    // Path 2 will upgrade attack speed from tower class

    [Header("Path 3")]
    public float stunDuration;
    public float stunChance;


    public override void BuySpecial(int pathIndex)
    {
        base.BuySpecial(pathIndex);

        if (specialUnlocked == 1)
        {
            damageType = DamageType.True;
        }
        else if (specialUnlocked == 2)
        {
            attackSpeed *= 2f;
        }
    }


    protected override void Attack(Enemy target)
    {
        target.TakeDamage(damage, damageType, this);

        if (specialUnlocked == 3 && Random.value < stunChance)
        {
            target.ApplySlow(1f, stunDuration);
        }
    }
}
