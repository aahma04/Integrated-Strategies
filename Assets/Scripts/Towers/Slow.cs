using UnityEngine;

public class Slow : Tower
{
    private void Attack(Enemy target)
    {
        if (specialUnlocked)
        {
            // spawn a circle and do attack on all enemies in it
        }
        else
        {
            target.TakeDamage(damage, damageType);
            target.AddEffect(null); // placeholder for slow effect
        }
    }
}
