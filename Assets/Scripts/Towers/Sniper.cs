using UnityEngine;

public class Sniper : Tower
{
    protected override void Attack(Enemy target)
    {
        target.TakeDamage(damage, (specialUnlocked == 1) ? DamageType.True : damageType, this);
    }
}
