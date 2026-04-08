using UnityEngine;

public class Slowed : Effect
{
    public float slowAmount;

    public Slowed(float duration, float slowAmount) : base(duration)
    {
        this.slowAmount = slowAmount;
    }

    public override void ApplyEffect(MonoBehaviour target)
    {
        Enemy enemyTarget = target as Enemy;
        if (enemyTarget != null)
        {
            // Slow enemy movement speed
        }
    }
    
}
