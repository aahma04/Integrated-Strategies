using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AttackRange : MonoBehaviour
{
    private Collider2D rangeCollider;
    private SpriteRenderer rangeIndicator;

    public List<Enemy> enemiesInRange;


    private void Awake()
    {
        rangeCollider = GetComponent<Collider2D>();
        rangeIndicator = GetComponent<SpriteRenderer>();
        SetIndicatorVisibility(false);

        enemiesInRange = new List<Enemy>();
    }


    public void SetRange(float newRange)
    {
        if (rangeCollider is CircleCollider2D circle)
        {
            circle.radius = newRange;
        }

        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = newRange * new Vector3(2f, 2f, 2f);
        }
    }


    public void SetIndicatorVisibility(bool visibility)
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = visibility;
        }
    }


    public void SetIndicatorColour(Color c)
    {
        rangeIndicator.color = c;
    }


    public Enemy[] GetTarget(int numTargets, Transform center, Tower.TargetPriority targetPriority)
    {

        if (enemiesInRange.Count == 0)
        {
            return null;
        }

        if (enemiesInRange.Count < numTargets)
        {
            numTargets = enemiesInRange.Count;
        }

        switch (targetPriority)
        {
            case Tower.TargetPriority.First:
                return enemiesInRange.OrderByDescending(e => e.trackProgress).Take(numTargets).ToArray();
            case Tower.TargetPriority.Last:
                return enemiesInRange.OrderBy(e => e.trackProgress).Take(numTargets).ToArray();
            case Tower.TargetPriority.Close:
                return enemiesInRange.OrderBy(e => Vector2.Distance(center.position, e.transform.position)).Take(numTargets).ToArray();
            case Tower.TargetPriority.Strong:
                return enemiesInRange.OrderByDescending(e => e.maxHP).Take(numTargets).ToArray();
        }

        return null;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collider enter: " + other.name);
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (!enemiesInRange.Contains(enemy))
                {
                    enemiesInRange.Add(enemy);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Enemy exited range: " + enemy.name);
                enemiesInRange.Remove(enemy);
            }
        }
    }
}
