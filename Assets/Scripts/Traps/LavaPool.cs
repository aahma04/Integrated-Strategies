using UnityEngine;

public class LavaPool : Trap
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage*Time.deltaTime, damageType, null);
            }
        }
    }
}
