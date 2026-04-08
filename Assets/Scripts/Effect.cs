using UnityEngine;

public class Effect
{
    public float duration;

    public Effect(float duration)
    {
        this.duration = duration;
    }

    public virtual void ApplyEffect(MonoBehaviour target) {}
}
