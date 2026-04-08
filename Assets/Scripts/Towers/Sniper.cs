using UnityEngine;

public class Sniper : Tower
{
    private void Attack(Enemy target)
    {
        target.TakeDamage(damage, specialUnlocked ? DamageType.True : damageType);
    }
}
