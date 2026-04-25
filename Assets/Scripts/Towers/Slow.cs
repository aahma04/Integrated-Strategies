using UnityEngine;
using System.Collections;

public class Slow : Tower
{
    [Header("Path 1")]
    public float slowRadius = 1.5f;
    private SpriteRenderer areaEffect;

    [Header("Path 2")]
    public float slowAmount = 0.4f;

    [Header("Path 3")]
    public float damageAmp;
    public float slowDuration = 5f;

    [Header("Perk Version")]
    public bool isAttackMode;
    public float attackModeDamage = 100f;
    public float attackModeAttackSpeed = 1f;
    public float attackModeSlowAmount = 0.15f;

    private GameObject attackNode;
    private AttackRange attackNodeScript;


    protected override void Awake()
    {
        base.Awake();
        attackNode = transform.Find("AttackNode").gameObject;
        attackNodeScript = attackNode.GetComponent<AttackRange>();
        attackNodeScript.SetRange(slowRadius);

        Transform AreaObject = transform.Find("AreaEffect");
        areaEffect = AreaObject.GetComponent<SpriteRenderer>();
        areaEffect.enabled = false;
    }


    public void EnterAttackMode()
    {
        damage = attackModeDamage;
        attackSpeed = attackModeAttackSpeed;
        slowAmount = attackModeSlowAmount;
        isAttackMode = true;
    }


    public override void BuySpecial(int pathIndex)
    {
        base.BuySpecial(pathIndex);

        if (specialUnlocked == 1)
        {
            attackNodeScript.SetRange(slowRadius);
        }
        else if (specialUnlocked == 2)
        {
            slowAmount += 0.3f;
        }
    }


    protected override void Attack(Enemy[] targets)
    {
        Enemy target = targets[0];
        
        if (specialUnlocked == 1)
        {
            attackNode.transform.position = target.transform.position;

            Enemy[] enemiesToHit = attackNodeScript.enemiesInRange.ToArray();

            foreach (Enemy enemy in enemiesToHit)
            {
                ApplyAttack(enemy);
            }
            areaEffect.gameObject.transform.localScale *= slowRadius;
            StartCoroutine(DoAreaAttackEffect(areaEffect, target, 0.5f));
        }
        else
        {
            ApplyAttack(target);
            StartCoroutine(DoAttackEffect(attackEffect, target));
        }

    }


    private void ApplyAttack(Enemy target)
    {
        target.TakeDamage(damage, damageType, this);

        target.ApplySlow(slowAmount, slowDuration);
        
        if (specialUnlocked == 3)
        {
            target.ApplyWeakness(damageAmp, slowDuration);
        }
        
    }


    public IEnumerator DoAreaAttackEffect(SpriteRenderer effectSprite, Enemy target, float duration=0.1f)
    {
        Transform effectObject = effectSprite.gameObject.transform;


        effectObject.position = target.transform.position;
        effectSprite.enabled = true;

        Vector3 startScale = new Vector3(0f, 0f, 0f);
        Vector3 toScale = new Vector3(1f, 1f, 1f)*slowRadius*2;

        float counter = 0f;

        while (counter < (duration/2))
        {
            counter += Time.deltaTime;
            effectObject.localScale = Vector3.Lerp(startScale, toScale, Mathf.Sqrt(counter / (duration/2)));
            yield return null;
        }
        yield return new WaitForSeconds(duration/2);
        effectSprite.enabled = false;
    }
}
