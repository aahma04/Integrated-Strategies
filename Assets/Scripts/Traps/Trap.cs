using UnityEngine;

public class Trap : MonoBehaviour
{
    public enum PlacementType
    {
        Path,
        Field,
        Any
    }

    [Header("Basic Information")]
    public string trapName;
    public string trapType;
    public string description;
    public int cost;
    public Color trapColor;

    [Header("Basic Attributes")]
    public float damage = 1f;
    public float range = 1f;
    public float duration = 1f;
    public Tower.DamageType damageType;
    public PlacementType placementType;
    public bool onGround = false;

    protected AttackRange attackRange;


    protected void Awake()
    {
        attackRange = GetComponentInChildren<AttackRange>();
        if (attackRange != null)
        {
            attackRange.SetRange(range);
        }
    }


    protected void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            TimeOut();
            Destroy(gameObject);
        }
    }


    protected virtual void OnStep(Enemy target)
    {
        return;
    }


    protected virtual void TimeOut()
    {
        return;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (onGround && enemy.flying)
                {
                    return;
                }

                OnStep(enemy);
            }
        }
    }
}
