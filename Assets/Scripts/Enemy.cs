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

    [Header("Pathing")]
    public float trackProgress = 0f;
    public float waypointReachedDistance = 0.05f;

    public List<Effect> activeEffects;

    private List<Vector3> path;
    private int currentPathIndex = 0;
    private bool hasPath = false;

    public void Start()
    {
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
        MoveAlongPath();

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].ApplyEffect(this);
        }
    }

    void MoveAlongPath()
    {
        if (!hasPath || path == null || path.Count == 0)
        {
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
        // Replace this later with base damage / life loss logic
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

    public void TakeDamage(float amount, Tower.DamageType damageType, Tower source)
    {
        if (energyImmune && damageType == Tower.DamageType.Energy)
        {
            return;
        }

        if (damageType == Tower.DamageType.Physical)
        {
            amount -= defense;
        }

        if (damageType != Tower.DamageType.True)
        {
            amount -= amount * (damageReduction / 100f);
        }

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
            if (source != null)
            {
                source.enemiesInRange.Remove(this);
            }

            Destroy(gameObject);
        }
    }

    public void AddEffect(Effect effect)
    {
        activeEffects.Add(effect);
    }
}