using UnityEngine;
using System.Collections;

public class Flamethrower : Tower
{
    public float burnDuration;

    private GameObject attackNode;
    private BoxCollider2D attackCollider;


    protected override void Start()
    {
        base.Start();
        attackNode = transform.Find("AttackNode").gameObject;
        attackCollider = attackNode.GetComponent<BoxCollider2D>();
    }


    protected override void Attack(Enemy target)
    {
        attackNode.transform.right = target.transform.position - transform.position;

        foreach (GameObject enemyObj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (attackCollider.bounds.Intersects(enemy.GetComponent<Collider2D>().bounds))
            {
                ApplyBurn(enemy);
            }
        }
    }


    private void ApplyBurn(Enemy target)
    {
        // Check if target is either not burning or if special unlocked (stacking enabled)
        if (!target.activeEffects.Exists(effect => effect is Burning) || specialUnlocked)
        {
            target.AddEffect(new Burning(damage, burnDuration, this));
        }
    }
}
