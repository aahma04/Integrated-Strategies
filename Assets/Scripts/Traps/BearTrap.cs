using UnityEngine;

public class BearTrap : Trap
{
    public float stunDuration;

    protected override void OnStep(Enemy target)
    {
        target.TakeDamage(damage, damageType, null);
        target.ApplySlow(1f, stunDuration);
        Destroy(gameObject);
    }
}
