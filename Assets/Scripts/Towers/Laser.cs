using UnityEngine;

public class Laser : Tower
{
    private float damageAmp = 1f;

    private Enemy currentTarget;

    private void Attack(Enemy target)
    {
        if (currentTarget != target)
        {
            damageAmp = 1f;
            currentTarget = target;
        }

        target.TakeDamage(damage * damageAmp, damageType);

        if (specialUnlocked)
        {
            damageAmp = Mathf.Min(damageAmp + (Time.deltaTime * 0.5f), 2.5f);
        }
    }
}
