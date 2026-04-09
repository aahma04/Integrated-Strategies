using UnityEngine;

public class Burning : Effect
{
    private float damagePerSecond;
    private Tower source;
    
    public Burning(float damagePerSecond, float duration, Tower source) : base(duration)
    {
        this.damagePerSecond = damagePerSecond;
        this.source = source;
    }

    public override void ApplyEffect(MonoBehaviour target)
    {
        Enemy enemyTarget = target as Enemy;
        if (enemyTarget != null)
        {
            enemyTarget.TakeDamage(damagePerSecond * Time.deltaTime, Tower.DamageType.True, source);

            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                enemyTarget.activeEffects.Remove(this);
            }
        }
    }
}
