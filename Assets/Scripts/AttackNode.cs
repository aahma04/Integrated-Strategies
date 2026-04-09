using UnityEngine;
using System.Collections.Generic;

public class AttackNode : MonoBehaviour
{
    public Collider2D attackCollider;

    public List<Enemy> enemiesInArea;


    private void Start()
    {
        attackCollider = GetComponent<Collider2D>();

        enemiesInArea = new List<Enemy>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collider enter: " + other.name);
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (!enemiesInArea.Contains(enemy))
                {
                    enemiesInArea.Add(enemy);
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
                enemiesInArea.Remove(enemy);
            }
        }
    }
}
