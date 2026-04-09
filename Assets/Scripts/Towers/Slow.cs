using UnityEngine;

public class Slow : Tower
{
    public float slowDuration = 5f;
    public float slowAmount = 0.6f;
    public float slowRadius = 1.5f;

    private GameObject attackNode;
    private CircleCollider2D attackCollider;
    private AttackNode attackNodeScript;

    private Enemy currentTarget;


    // protected override void Start()
    // {
    //     base.Start();
    //     attackNode = transform.Find("AttackNode").gameObject;
    //     attackCollider = attackNode.GetComponent<CircleCollider2D>();
    //     attackCollider.radius = slowRadius;

    //     specialUnlocked = true; // for testing
    //     attackNodeScript = attackNode.GetComponent<AttackNode>();
    // }

    // protected override void Update()
    // {
    //     base.Update();
    //     attackNode.transform.position = ;
    // }

    protected override void Attack(Enemy target)
    {
        if (specialUnlocked)
        {
            attackNode.transform.position = target.transform.position;

            // foreach (GameObject enemyObj in GameObject.FindGameObjectsWithTag("Enemy"))
            // {
            //     Enemy enemy = enemyObj.GetComponent<Enemy>();
            //     if (attackCollider.bounds.Contains(enemy.transform.position))
            //     {
            //         target.TakeDamage(damage, damageType, this);

            //         // Only apply slow if enemy isn't already slowed
            //         if (!enemy.activeEffects.Exists(effect => effect is Slowed))
            //         {
            //             enemy.AddEffect(new Slowed(slowDuration, slowAmount));
            //         }
            //     }
            //  }

            // attackNodeScript.attackCollider.enabled = true;
            // Enemy[] enemiesToHit = attackNodeScript.enemiesInArea.ToArray();
            // Debug.Log("Enemies hit by slow: " + enemiesToHit.Length);
            // attackNodeScript.attackCollider.enabled = false;

            // foreach (Enemy enemy in enemiesToHit)
            // {
            //     enemy.TakeDamage(damage, damageType, this);

            //     // Only apply slow if enemy isn't already slowed
            //     if (!enemy.activeEffects.Exists(effect => effect is Slowed))
            //     {
            //         enemy.AddEffect(new Slowed(slowDuration, slowAmount));
            //     }
            // }

            // Temp since aoe is broken
            target.TakeDamage(damage, damageType, this);

            // Only apply slow if enemy isn't already slowed
            if (!target.activeEffects.Exists(effect => effect is Slowed))
            {
                target.AddEffect(new Slowed(slowDuration, slowAmount));
            }

        }
        else
        {
            target.TakeDamage(damage, damageType, this);

            // Only apply slow if enemy isn't already slowed
            if (!target.activeEffects.Exists(effect => effect is Slowed))
            {
                target.AddEffect(new Slowed(slowDuration, slowAmount));
            }
            
        }
    }

}
