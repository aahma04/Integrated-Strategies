using UnityEngine;
using System.Collections.Generic;

public class AttackRange : MonoBehaviour
{
    public Collider2D rangeCollider;

    public List<Enemy> enemiesInRange;


    private void Start()
    {
        rangeCollider = GetComponent<Collider2D>();

        enemiesInRange = new List<Enemy>();
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
                enemiesInRange.Remove(enemy);
            }
        }
    }
}
