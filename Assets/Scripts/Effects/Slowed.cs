using UnityEngine;
using System.Collections;

public class Slowed : Effect
{
    public float slowAmount;

    private bool isApplied = false;

    public Slowed(float duration, float slowAmount) : base(duration)
    {
        this.slowAmount = slowAmount;
    }

    public override void ApplyEffect(MonoBehaviour target)
    {
        Enemy enemyTarget = target as Enemy;
        if (enemyTarget != null)
        {
            if (!isApplied)
            {
                enemyTarget.speed *= slowAmount;
                isApplied = true;
            }

            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                enemyTarget.speed /= slowAmount;
                enemyTarget.activeEffects.Remove(this);
            }
        }
    }
}
