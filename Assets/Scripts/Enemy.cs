using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [Header("Basic Information")]
    public string enemyName;
    public string enemyType;
    public string enemyDesc;

    [Header("Basic Attributes")]
    public float maxHP = 10f;
    public float currentHP;
    public float damageReduction = 0f;
    public float defense = 0f;
    public float damage = 1f;
    public float attackSpeed = 1f;
    public float speed = 1f;
    public bool flying = false;
    public bool energyImmune = false;
    public bool isBoss = false;

    [Header("Pathing")]
    public float trackProgress = 0f;
    public float waypointReachedDistance = 0.05f;

    [HideInInspector]
    public float speedModifier = 1f;
    [HideInInspector]
    public float speedModifierDuration = 0f;
    [HideInInspector]
    public float defenseModifier = 0f;
    [HideInInspector]
    public float defenseModifierDuration = 0f;
    [HideInInspector]
    public float damageModifier = 1f;
    [HideInInspector]
    public float damageModifierDuration = 0f;
    [HideInInspector]
    public float silenceDuration = 0f;

    [HideInInspector]
    public List<Effect> activeEffects;

    public List<Vector3> path;
    private int currentPathIndex = 0;
    private bool hasPath = false;

    private MapLoader mapLoader;
    private char targetEndSymbol;
    private IncomeTracker incomeTracker;

    public void Start()
    {
        incomeTracker = FindAnyObjectByType<IncomeTracker>();
        currentHP = maxHP;

        if (currentHP <= 0)
        {
            Destroy(gameObject);
            return;
        }

        activeEffects = new List<Effect>();
    }

    public void Update()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].ApplyEffect(this);
        }

        if (speedModifierDuration > 0)
        {
            speedModifierDuration -= Time.deltaTime;
            if (speedModifierDuration <= 0)
            {
                speedModifier = 1f;
            }
        }

        if (defenseModifierDuration > 0)
        {
            defenseModifierDuration -= Time.deltaTime;
            if (defenseModifierDuration <= 0)
            {
                defenseModifier = 0f;
            }
        }

        if (damageModifierDuration > 0)
        {
            damageModifierDuration -= Time.deltaTime;
            if (damageModifierDuration <= 0)
            {
                damageModifier = 1f;
            }
        }

        if (silenceDuration > 0)
        {
            silenceDuration -= Time.deltaTime;
        }

        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        if (!hasPath || path == null || path.Count == 0)
        {
            transform.position -= Vector3.right * speed * speedModifier * Time.deltaTime;
            return;
        }

        if (currentPathIndex >= path.Count)
        {
            ReachEnd();
            return;
        }

        Vector3 target = path[currentPathIndex];
        target.z = transform.position.z;

        Vector3 oldPosition = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        trackProgress += Vector3.Distance(oldPosition, transform.position);

        if (Vector3.Distance(transform.position, target) <= waypointReachedDistance)
        {
            currentPathIndex++;

            if (currentPathIndex >= path.Count)
            {
                ReachEnd();
            }
        }
    }

    void ReachEnd()
    {
        if (mapLoader != null)
        {
            mapLoader.DamageEndTile(targetEndSymbol, 1);
            mapLoader.NotifyEnemyRemoved();
        }
        else
        {
            Debug.LogWarning("Enemy reached end but mapLoader was not assigned.");
        }

        Destroy(gameObject);
    }

    public void SetPath(List<Vector3> newPath)
    {
        if (newPath == null || newPath.Count == 0)
        {
            Debug.LogError("Enemy received a null or empty path.");
            hasPath = false;
            return;
        }

        path = new List<Vector3>(newPath);
        currentPathIndex = 0;
        hasPath = true;

        Vector3 startPos = path[0];
        startPos.z = transform.position.z;
        transform.position = startPos;

        trackProgress = 0f;
    }

    public void TakeDamage(float amount, Tower.DamageType damageType, Tower source​)
    {
        if (energyImmune && damageType == Tower.DamageType.Energy)
        {
            return;
        }

        if (damageType == Tower.DamageType.Physical)
        {
            amount -= (defense - defenseModifier);
        }

        if (damageType != Tower.DamageType.True)
        {
            amount -= amount * (damageReduction / 100f);
        }

        amount *= damageModifier;

        if (amount <= 0)
        {
            currentHP -= 1f;
        }
        else
        {
            currentHP -= amount;
        }

        Debug.Log("Damage taken: " + amount + " Current HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die(source);
        }
    }


    public void Die(Tower source)
    {
        if (source != null)
        {
            source.attackRange.enemiesInRange.Remove(this);
        }

        if (incomeTracker != null)
        {
            incomeTracker.currentMoney += 25;
        }
        if (mapLoader != null)
        {
            mapLoader.NotifyEnemyRemoved();
        }
        else
        {
            Debug.LogWarning("IncomeTracker not found in scene.");
        }
        Destroy(gameObject);
    }


    public void ApplySlow(float amount, float duration)
    {
        speedModifier = Mathf.Min(speedModifier, 1 - amount);
        speedModifierDuration = Mathf.Max(speedModifierDuration, duration);
    }


    public void ApplyShred(float amount, float duration)
    {
        defenseModifier = Mathf.Max(defenseModifier, amount);
        defenseModifierDuration = Mathf.Max(defenseModifierDuration, duration);
    }


    public void ApplyWeakness(float amount, float duration)
    {
        damageModifier = Mathf.Min(damageModifier, amount);
        damageModifierDuration = Mathf.Max(damageModifierDuration, duration);
    }


    public void ApplySilence(float duration)
    {
        silenceDuration = Mathf.Max(silenceDuration, duration);
    }


    public void AddEffect(Effect effect)
    {
        activeEffects.Add(effect);
    }

    public void SetMapLoader(MapLoader loader)
    {
        mapLoader = loader;
    }

    public void SetTargetEnd(char endSymbol)
    {
        targetEndSymbol = endSymbol;
    }
}