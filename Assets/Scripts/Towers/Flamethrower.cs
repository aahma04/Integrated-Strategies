using UnityEngine;

public class Flamethrower : Tower
{
    public float duration;

    // Attack should spawn a rectangle collider and apply fire effect to enemies in it, do this later
    private void Attack(Enemy target)
    {
        // Check if target is either not burning or if special unlocked (stacking enabled)
        if (!target.activeEffects.Exists(effect => effect is Burning) && specialUnlocked)
        {
            target.AddEffect(new Burning(damage, duration));
        }
    }
}
