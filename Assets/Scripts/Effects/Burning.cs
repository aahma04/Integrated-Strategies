using UnityEngine;

public class Burning : Effect
{
    private float damagePerSecond;
    
    public Burning(float damagePerSecond, float duration) : base(duration)
    {
        this.damagePerSecond = damagePerSecond;
    }

    public override void ApplyEffect(MonoBehaviour target)
    {
        Enemy enemyTarget = target as Enemy;
        if (enemyTarget != null)
        {
            enemyTarget.TakeDamage(damagePerSecond * Time.deltaTime, Tower.DamageType.True);
        }
    }
}
