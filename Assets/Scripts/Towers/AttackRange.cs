using UnityEngine;
using System.Collections.Generic;

public class AttackRange : MonoBehaviour
{
    private Collider2D rangeCollider;

    public List<Enemy> enemiesInRange;


    private void Start()
    {
        rangeCollider = GetComponent<Collider2D>();

        enemiesInRange = new List<Enemy>();
    }


    public void SetRange(float newRange)
    {
        if (rangeCollider is CircleCollider2D circle)
        {
            circle.radius = newRange;
        }
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
