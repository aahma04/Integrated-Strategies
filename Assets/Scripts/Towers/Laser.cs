using UnityEngine;
using System.Collections.Generic;

public class Laser : Tower
{
    public Color baseAttackColour;
    public Color amplifiedAttackColour;

    [Header("Path 1")]
    private float damageAmp = 1f;
    public float damageAmpCap = 2.5f;
    public float damageRampRate = 0.5f;
    
    [Header("Path 2")]
    public float special2SlowAmount = 0.5f;

    [Header("Path 3")]
    public float chainDamageFalloff;
    public float chainRange;
    public int numBounces;

    private SpriteRenderer bounceEffect;
    private Transform bounceEffectObject;

    private List<Enemy> bounceTargets;


    private Enemy currentTarget;

    private GameObject attackNode;
    private AttackRange attackNodeScript;

    private Transform attackEffectObject;


    protected override void Awake()
    {
        base.Awake();
        attackNode = transform.Find("AttackNode").gameObject;
        attackNodeScript = attackNode.GetComponent<AttackRange>();
        attackNodeScript.SetRange(chainRange);

        attackEffectObject = attackEffect.gameObject.transform;


        bounceEffectObject = transform.Find("BounceEffect");
        bounceEffect = bounceEffectObject.GetComponent<SpriteRenderer>();
        bounceEffect.color = baseAttackColour;
        bounceEffect.enabled = false;

        bounceTargets = new List<Enemy>();
    }


    protected override void Update()
    {
        base.Update();

        if (specialUnlocked == 1)
        {
            damageAmp = Mathf.Min(damageAmp + (Time.deltaTime * damageRampRate), damageAmpCap);
        }

        if (attackRange.enemiesInRange.Count == 0 || !attackRange.enemiesInRange.Contains(currentTarget))
        {
            attackEffect.enabled = false;
            if (specialUnlocked == 3)
            {
                bounceEffect.enabled = false;
            }
        }
        else
        {
            attackEffectObject.right = currentTarget.transform.position - attackEffectObject.position;
            attackEffectObject.localScale = new Vector3(Vector3.Distance(attackEffectObject.position, currentTarget.transform.position), 0.1f, 1f);

            attackEffect.color = specialUnlocked==1 ? Color.Lerp(baseAttackColour, amplifiedAttackColour, (damageAmp-1)/(damageAmpCap-1)) : baseAttackColour;

            attackEffect.enabled = true;

            if (specialUnlocked == 3 && bounceTargets.Count > 0)
            {
                Vector3 startPos = currentTarget.transform.position;
                Vector3 endPos = bounceTargets[0].transform.position;

                bounceEffectObject.position = startPos;
                bounceEffectObject.right = endPos - startPos;

                bounceEffectObject.localScale = new Vector3(Vector3.Distance(startPos, endPos), 0.1f, 1f);

                bounceEffect.enabled = true;
            }
        }
    }


    protected override void Attack(Enemy[] targets)
    {
        Enemy target = targets[0];

        if (currentTarget != target)
        {
            damageAmp = 1f;
            currentTarget = target;
        }

        target.TakeDamage(damage * damageAmp, damageType, this);

        if (specialUnlocked == 2)
        {
            target.ApplySlow(special2SlowAmount, 1/attackSpeed);
        }

        if (specialUnlocked == 3)
        {
            bounceTargets = new List<Enemy>();
            Enemy bounceSource = currentTarget;
            float bounceDamage = damage;

            attackNodeScript.SetRange(chainRange);

            for (int i=0; i<numBounces; i++)
            {
                attackNode.transform.position = bounceSource.transform.position;
                Enemy[] bounceSearch = attackNodeScript.GetTarget(1, bounceSource.transform, TargetPriority.Close, new List<Enemy>() {currentTarget});
                if (bounceSearch == null || bounceSearch.Length <= 0)
                {
                    break;
                }
                Enemy enemyToHit = bounceSearch[0];
                if (enemyToHit == null)
                {
                    break;
                }
                bounceTargets.Add(enemyToHit);
                bounceDamage *= chainDamageFalloff;
                Debug.Log("Bouncing to: "+enemyToHit.enemyName+" for "+bounceDamage+" damage | bouncetargets: "+bounceTargets.Count);
                enemyToHit.TakeDamage(bounceDamage, damageType, this);
                bounceSource = enemyToHit;
            } 
        }
    }
}
