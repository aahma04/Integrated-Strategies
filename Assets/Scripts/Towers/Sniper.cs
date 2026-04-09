using UnityEngine;

public class Sniper : Tower
{
    protected override void Attack(Enemy target)
    {
        target.TakeDamage(damage, specialUnlocked ? DamageType.True : damageType, this);
    }
}
