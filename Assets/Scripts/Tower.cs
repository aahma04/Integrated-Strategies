using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Basic Information")]
    public string towerName;
    public string towerType;
    public string desc;

    [Header("Basic Attributes")]
    public float maxHP = 10f;
    public float dmgRed = 0f; //Percentage
    public float defense = 0f; //Flat
    public float damage = 1f;
    public float attackSpd = 1f;
    public float range = 0f;

    [Header("Attack Info")]
    public string dmgType;
    public string projType;
    public bool highTile = true;
    public bool lowTile = true;
    public float skillCooldown = 1f;
    public float skillDmg = 10f;

    [Header("Type")]
    public string attackType; //"Single" , "AOE", etc. might be changed later
}
