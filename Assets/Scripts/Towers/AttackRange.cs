using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AttackRange : MonoBehaviour
{
    private Collider2D rangeCollider;
    private SpriteRenderer rangeIndicator;

    [HideInInspector]
    public List<Enemy> enemiesInRange;

    [HideInInspector]
    public List<GameObject> tilesInRange;

    public bool checkTiles;


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
            Debug.Log($"indicator enabled: {rangeIndicator.enabled}");
        }
    }


    public void SetIndicatorColour(Color c)
    {
        rangeIndicator.color = c;
    }


    public Enemy[] GetTarget(int numTargets, Transform center, Tower.TargetPriority targetPriority, List<Enemy> exclude=null)
    {

        if (enemiesInRange.Count == 0)
        {
            return null;
        }

        if (enemiesInRange.Count < numTargets)
        {
            numTargets = enemiesInRange.Count;
        }

        List<Enemy> candidates = exclude != null ? enemiesInRange.Except(exclude).ToList() : enemiesInRange;

        switch (targetPriority)
        {
            case Tower.TargetPriority.First:
                return candidates.OrderByDescending(e => e.trackProgress).Take(numTargets).ToArray();
            case Tower.TargetPriority.Last:
                return candidates.OrderBy(e => e.trackProgress).Take(numTargets).ToArray();
            case Tower.TargetPriority.Close:
                return candidates.OrderBy(e => Vector2.Distance(center.position, e.transform.position)).Take(numTargets).ToArray();
            case Tower.TargetPriority.Strong:
                return candidates.OrderByDescending(e => e.maxHP).Take(numTargets).ToArray();
        }

        return null;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
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

        if (other.CompareTag("Path") && checkTiles)
        {
            GameObject tile = other.gameObject;
            if (tile != null)
            {
                if (!tilesInRange.Contains(tile))
                {
                    tilesInRange.Add(tile);
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
                enemiesInRange.Remove(enemy);
            }
        }
    }
}
