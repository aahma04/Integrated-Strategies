using UnityEngine;
using System.Collections.Generic;

public class PerkManager : MonoBehaviour
{
    [Header("Trap Prefabs (for random trap perk)")]
    public List<GameObject> nonNukeTrapPrefabs;

    [Header("References")]
    public TowerPlacementManager placementManager;
    public IncomeTracker incomeTracker;
    public MapLoader mapLoader;

    // Enemy modifiers (applied at spawn)
    [HideInInspector] public float enemyHealthMultiplier = 1f;

    // Tower modifiers (applied at placement)
    [HideInInspector] public float areaAttackSpeedBonus = 1f;

    // Lvl 15: enemies move 20% slower
    [HideInInspector] public bool slowTierActive = false;

    // Lvl 20: all towers deal 20% more damage while a boss is alive
    [HideInInspector] public float globalDamageVsBossMultiplier = 1f;

    // Lvl 25: gold generation +15%
    [HideInInspector] public float goldGenerationBonus = 1f;

    private bool startingTowerPlaced = false;

    // Tracks whether a boss is currently alive (towers check this)
    public static bool bossAlive = false;

    void Awake()
    {
        ApplyPerks(PlayerProgress.playerLevel);
    }

    void ApplyPerks(int level)
    {
        // ── Level 5: traps 35% cheaper (applied via GetTrapCost()) ────────

        // ── Level 10: area towers attack 20% faster ────────────────────────
        if (level >= 10)
            areaAttackSpeedBonus = 1.2f;

        // ── Level 15: enemies move 20% slower ─────────────────────────────
        if (level >= 15)
            slowTierActive = true;

        // ── Level 20: +20% damage vs bosses ───────────────────────────────
        if (level >= 20)
            globalDamageVsBossMultiplier = 1.2f;

        // ── Level 25: gold +15% ────────────────────────────────────────────
        if (level >= 25)
            goldGenerationBonus = 1.15f;
    }

    // Called by TowerPlacementManager after placing any tower.
    public void ApplyToTower(Tower tower)
    {
        if (tower == null) return;

        // Lvl 10: area towers attack 20% faster
        if (PlayerProgress.playerLevel >= 10 && tower.projectileType == Tower.ProjectileType.Area)
            tower.attackSpeed *= areaAttackSpeedBonus;

        // Lvl 15: Slow tower enters attack mode
        if (slowTierActive && tower is Slow slowTower)
            slowTower.EnterAttackMode();
    }

    // Called by MapLoader when spawning an enemy.
    public void ApplyToEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        enemy.maxHP *= enemyHealthMultiplier;
        enemy.currentHP = enemy.maxHP;

        // Lvl 15: enemies move 20% slower
        if (slowTierActive)
            enemy.speed *= 0.8f;

        // Track boss presence for lvl 20 perk
        if (enemy.isBoss)
            bossAlive = true;
    }

    // Called by Enemy.Die() to notify boss is gone.
    public void NotifyBossDied()
    {
        // Check if any other bosses remain
        foreach (Enemy e in FindObjectsOfType<Enemy>())
        {
            if (e.isBoss) return;
        }
        bossAlive = false;
    }

    // Called by Enemy.Die() to get the final gold amount with bonuses applied.
    public int ApplyGoldBonus(int baseAmount)
    {
        return Mathf.RoundToInt(baseAmount * goldGenerationBonus);
    }

    // Returns adjusted trap cost (lvl 5: 35% cheaper).
    public int GetTrapCost(int baseCost)
    {
        if (PlayerProgress.playerLevel >= 5)
            return Mathf.RoundToInt(baseCost * 0.65f);
        return baseCost;
    }

    // ── Starting tower placement ──────────────────────────────────────────
    public void PlaceStartingTower()
    {
        if (startingTowerPlaced || placementManager == null) return;

        GameObject prefabToPlace = GetStartingTowerPrefab();
        if (prefabToPlace == null) return;

        // Find the index of this prefab in the placement manager's list
        int index = placementManager.towerPrefabs.IndexOf(prefabToPlace);
        if (index < 0)
        {
            Debug.LogWarning("PerkManager: Starting tower prefab not found in TowerPlacementManager list.");
            return;
        }

        // Temporarily set cost to 0 so the player places it for free
        Tower towerScript = prefabToPlace.GetComponent<Tower>();
        int originalCost = 0;
        if (towerScript != null)
        {
            originalCost = towerScript.cost;
            towerScript.cost = 0;
        }

        // Give it to the player as a pre-selected tower in hand
        placementManager.SelectFreeStartingTower(index, originalCost, this);

        // Lvl 5: also give a random non-nuke trap
        if (PlayerProgress.playerLevel >= 5 && nonNukeTrapPrefabs != null && nonNukeTrapPrefabs.Count > 0)
        {
            int idx = Random.Range(0, nonNukeTrapPrefabs.Count);
            placementManager.trapPrefabs.Add(nonNukeTrapPrefabs[idx]);
        }

        startingTowerPlaced = true;
    }

    // Called by TowerPlacementManager after the free tower is placed to restore cost.
    public void RestoreStartingTowerCost(GameObject prefab, int originalCost)
    {
        Tower towerScript = prefab.GetComponent<Tower>();
        if (towerScript != null)
            towerScript.cost = originalCost;
    }

    GameObject GetStartingTowerPrefab()
    {
        int level = PlayerProgress.playerLevel;
        if (level >= 25) return GetTowerPrefabByType<Laser>();
        if (level >= 20) return GetTowerPrefabByType<Flamethrower>();
        if (level >= 15) return GetTowerPrefabByType<Slow>();
        if (level >= 10) return GetTowerPrefabByType<Splash>();
        if (level >= 5) return GetTowerPrefabByType<Sniper>();
        return GetTowerPrefabByType<RapidFire>();
    }

    GameObject GetTowerPrefabByType<T>() where T : Tower
    {
        foreach (GameObject prefab in placementManager.towerPrefabs)
        {
            if (prefab != null && prefab.GetComponent<T>() != null)
                return prefab;
        }
        Debug.LogWarning($"PerkManager: Could not find prefab for type {typeof(T).Name}");
        return null;
    }


}