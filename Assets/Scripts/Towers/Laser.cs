using UnityEngine;

public class Laser : Tower
{
    [Header("Path 1")]
    public float damageRampRate = 0.5f;
    
    [Header("Path 2")]
    public float special2SlowAmount = 0.5f;

    [Header("Path 3")]
    public float special3DamageFalloff;
    public float special3Range;
    public int special3NumBounces;

    private float damageAmp = 1f;

    private Enemy currentTarget;

    protected override void Attack(Enemy target)
    {
        if (currentTarget != target)
        {
            damageAmp = 1f;
            currentTarget = target;
        }

        target.TakeDamage(damage * damageAmp, damageType, this);

        if (specialUnlocked == 1)
        {
            damageAmp = Mathf.Min(damageAmp + (Time.deltaTime * damageRampRate), 2.5f);
        }

        if (specialUnlocked == 2)
        {
            target.TakeDamage(damage, damageType, this);
            target.ApplySlow(special2SlowAmount, 1/attackSpeed);
        }

        if (specialUnlocked == 3)
        {
            // Do chaining
        }
    }
}
