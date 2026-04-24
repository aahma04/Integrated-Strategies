using UnityEngine;

public class Nuke : Trap
{
    protected override void TimeOut()
    {
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy enemy = e.GetComponent<Enemy>();

            if (!enemy.isBoss)
            {
                enemy.Die(null);
            }
        }
    }
}
