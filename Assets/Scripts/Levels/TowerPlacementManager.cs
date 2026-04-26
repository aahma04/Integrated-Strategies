using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class TowerPrefabEntry
{
    public string towerName;
    public GameObject prefab;
}

public class TowerPlacementManager : MonoBehaviour
{
    [Header("References")]
    public MapLoader mapLoader;
    public Camera mainCamera;
    public TowerInspector towerInspector;
    public PerkManager perkManager;

    [Header("Tower Prefabs")]
    public List<GameObject> towerPrefabs = new();
    public List<GameObject> trapPrefabs = new();

    [Header("Preview")]
    public Color validPreviewColor = new Color(0f, 1f, 0f, 0.45f);
    public Color invalidPreviewColor = new Color(1f, 0f, 0f, 0.45f);
    public Color validRangeColor = new Color(0f, 0.7f, 1f, 0.18f);
    public Color invalidRangeColor = new Color(1f, 0.2f, 0.2f, 0.18f);

    private int selectedTowerIndex = -1;
    private int selectedTrapIndex = -1;
    private bool placingTrap = false;
    private GameObject previewObject;
    private RangeCirclePreview rangePreview;

    private readonly Dictionary<Vector2Int, TowerInstance> placedTowers = new();

    // Free starting tower state
    private bool placingFreeStartingTower = false;
    private int freeStartingTowerOriginalCost = 0;
    private GameObject freeStartingTowerPrefab = null;
    private PerkManager freeStartingTowerPerkManager = null;

    void Start()
    {
        CreateRangePreview();
    }

    void Update()
    {
        if (mainCamera == null || mapLoader == null)
            return;

        bool hasSelection = placingTrap
            ? (selectedTrapIndex >= 0 && selectedTrapIndex < trapPrefabs.Count)
            : (selectedTowerIndex >= 0 && selectedTowerIndex < towerPrefabs.Count);

        if (!hasSelection)
        {
            HidePreviews();
            return;
        }

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        if (!mapLoader.TryGetGridPositionFromWorld(mouseWorld, out Vector2Int gridPos))
        {
            HidePreviews();
            return;
        }

        Vector3 snappedPos = mapLoader.GetSnappedWorldPosition(gridPos);

        bool isValid = placingTrap ? IsValidTrapPlacement(gridPos) : IsValidPlacement(gridPos);
        float previewRange = placingTrap ? GetSelectedTrapRange() : GetSelectedTowerRange();

        ShowPreviewAt(snappedPos, isValid);
        ShowRangeAt(snappedPos, previewRange, isValid);

        if (Input.GetMouseButtonDown(0))
        {
            if (placingTrap)
                TryPlaceSelectedTrap(gridPos, snappedPos);
            else
                TryPlaceSelectedTower(gridPos, snappedPos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClearSelection();
        }
    }

    public void SelectTowerByIndex(int index)
    {
        Debug.Log(index);
        if (index < 0 || index >= towerPrefabs.Count)
        {
            Debug.LogWarning($"Invalid tower index: {index}");
            return;
        }

        selectedTowerIndex = index;
        RebuildPreviewObject();
    }

    public void SelectFreeStartingTower(int index, int originalCost, PerkManager perkManager)
    {
        placingFreeStartingTower = true;
        freeStartingTowerOriginalCost = originalCost;
        freeStartingTowerPrefab = towerPrefabs[index];
        freeStartingTowerPerkManager = perkManager;
        SelectTowerByIndex(index);
    }

    public void SelectTrapByIndex(int index)
    {
        if (index < 0 || index >= trapPrefabs.Count)
        {
            Debug.LogWarning($"Invalid trap index: {index}");
            return;
        }

        selectedTrapIndex = index;
        selectedTowerIndex = -1;
        placingTrap = true;
        RebuildPreviewObject(trapPrefabs[index]);
    }

    public void ClearSelection()
    {
        selectedTowerIndex = -1;
        selectedTrapIndex = -1;
        placingTrap = false;

        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        HidePreviews();
    }

    bool IsValidTrapPlacement(Vector2Int gridPos)
    {
        if (!mapLoader.IsValidGridPosition(gridPos)) return false;
        if (selectedTrapIndex < 0 || selectedTrapIndex >= trapPrefabs.Count) return false;

        Trap trap = trapPrefabs[selectedTrapIndex].GetComponent<Trap>();
        if (trap == null) return false;

        Debug.Log($"Placement type: {trap.placementType}");
        switch (trap.placementType)
        {
            case Trap.PlacementType.Path:
                return mapLoader.IsPathTile(gridPos) || mapLoader.IsStartOrEndTile(gridPos);
            case Trap.PlacementType.Field:
                return mapLoader.IsGrassTile(gridPos);
            case Trap.PlacementType.Any:
                return true;
            default:
                return false;
        }
    }

    void TryPlaceSelectedTrap(Vector2Int gridPos, Vector3 snappedPos)
    {
        if (!IsValidTrapPlacement(gridPos)) return;

        GameObject trapPrefab = trapPrefabs[selectedTrapIndex];
        if (trapPrefab == null) return;

        Trap trapScript = trapPrefab.GetComponent<Trap>();
        int cost = trapScript != null ? trapScript.cost : 0;

        if (perkManager != null)
            cost = perkManager.GetTrapCost(cost);

        if (towerInspector.incomeTracker.currentMoney < cost) return;

        towerInspector.incomeTracker.currentMoney -= cost;

        Instantiate(trapPrefab, snappedPos, Quaternion.identity);
        ClearSelection();
    }

    public void TryPlaceTrap(Vector2Int gridPos, Vector3 snappedPos, GameObject trapPrefab)
    {
        int temp_selectedTrapIndex = selectedTrapIndex;

        selectedTrapIndex = 3;

        // Debug.Log("beginning trap placement");
        // if (!IsValidTrapPlacement(gridPos)) return;

        // Debug.Log("after IsValidTrapPlacement");
        // if (trapPrefab == null) return;

        // Debug.Log("after trapPrefab == null");
        // Trap trapScript = trapPrefab.GetComponent<Trap>();
        // int cost = trapScript != null ? trapScript.cost : 0;

        // if (perkManager != null)
        //     cost = perkManager.GetTrapCost(cost);

        // if (towerInspector.incomeTracker.currentMoney < cost) return;
        // Debug.Log("after money check");

        // towerInspector.incomeTracker.currentMoney -= cost;

        // Instantiate(trapPrefab, snappedPos, Quaternion.identity);

        TryPlaceSelectedTrap(gridPos, snappedPos);

        selectedTrapIndex = temp_selectedTrapIndex;
    }

    float GetSelectedTrapRange()
    {
        if (selectedTrapIndex < 0 || selectedTrapIndex >= trapPrefabs.Count) return 0f;
        GameObject prefab = trapPrefabs[selectedTrapIndex];
        if (prefab == null) return 0f;
        Trap trap = prefab.GetComponent<Trap>();
        return trap != null ? trap.range : 0f;
    }

    bool IsValidPlacement(Vector2Int gridPos)
    {
        if (!mapLoader.IsValidGridPosition(gridPos))
            return false;

        if (!mapLoader.IsPlaceableTile(gridPos))
            return false;

        if (placedTowers.ContainsKey(gridPos))
            return false;

        return true;
    }

    void TryPlaceSelectedTower(Vector2Int gridPos, Vector3 snappedPos)
    {
        if (!IsValidPlacement(gridPos))
            return;

        GameObject towerPrefab = towerPrefabs[selectedTowerIndex];

        if (towerPrefab == null)
        {
            Debug.LogWarning("Selected tower prefab is null.");
            return;
        }

        if (towerInspector.incomeTracker.currentMoney < towerPrefab.GetComponent<Tower>().cost)
        {
            return;
        }

        towerInspector.incomeTracker.currentMoney -= towerPrefab.GetComponent<Tower>().cost;

        GameObject placedTower = Instantiate(towerPrefab, snappedPos, Quaternion.identity);

        TowerInstance towerInstance = placedTower.GetComponent<TowerInstance>();
        if (towerInstance == null)
        {
            towerInstance = placedTower.AddComponent<TowerInstance>();
            towerInspector.SelectTower(placedTower.GetComponent<Tower>());
        }

        towerInstance.Initialize(gridPos, towerInspector);
        placedTowers[gridPos] = towerInstance;

        Tower tower = placedTower.GetComponent<Tower>();
        if (perkManager != null && tower != null)
            perkManager.ApplyToTower(tower);

        // Restore the prefab's original cost after free placement
        if (placingFreeStartingTower && freeStartingTowerPerkManager != null)
        {
            freeStartingTowerPerkManager.RestoreStartingTowerCost(freeStartingTowerPrefab, freeStartingTowerOriginalCost);
            placingFreeStartingTower = false;
            freeStartingTowerPrefab = null;
            freeStartingTowerPerkManager = null;
        }

        ClearSelection();
    }

    void CreateRangePreview()
    {
        GameObject obj = new GameObject("RangePreview");
        rangePreview = obj.AddComponent<RangeCirclePreview>();
        rangePreview.Show(false);
    }

    void ShowRangeAt(Vector3 position, float radius, bool isValid)
    {
        if (rangePreview == null) return;

        rangePreview.transform.position = new Vector3(position.x, position.y, 0.1f);
        rangePreview.SetRadius(radius);
        rangePreview.SetColor(isValid ? validRangeColor : invalidRangeColor);
        rangePreview.Show(true);
    }

    void ShowPreviewAt(Vector3 position, bool isValid)
    {
        if (previewObject == null)
        {
            RebuildPreviewObject(placingTrap && selectedTrapIndex >= 0 && selectedTrapIndex < trapPrefabs.Count
                ? trapPrefabs[selectedTrapIndex] : null);
        }

        if (previewObject == null)
            return;

        previewObject.transform.position = new Vector3(position.x, position.y, 0f);
        SetPreviewAlpha(previewObject, isValid ? validPreviewColor : invalidPreviewColor);
        previewObject.SetActive(true);
    }

    void HidePreviews()
    {
        if (previewObject != null)
        {
            previewObject.SetActive(false);
        }

        if (rangePreview != null)
        {
            rangePreview.Show(false);
        }
    }

    void RebuildPreviewObject(GameObject prefabOverride = null)
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        GameObject prefab = prefabOverride;
        if (prefab == null)
        {
            if (selectedTowerIndex < 0 || selectedTowerIndex >= towerPrefabs.Count)
                return;
            prefab = towerPrefabs[selectedTowerIndex];
        }

        if (prefab == null)
            return;

        previewObject = Instantiate(prefab);
        previewObject.name = prefab.name + "_Preview";

        DisableColliders(previewObject);
        SetPreviewAlpha(previewObject, validPreviewColor);
    }

    void DisableColliders(GameObject obj)
    {
        Collider2D[] colliders2D = obj.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D c in colliders2D)
        {
            c.enabled = false;
        }

        Collider[] colliders3D = obj.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders3D)
        {
            c.enabled = false;
        }
    }

    void SetPreviewAlpha(GameObject obj, Color tint)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in renderers)
        {
            sr.color = tint;
            sr.sortingOrder = 100;
        }
    }

    float GetSelectedTowerRange()
    {
        if (selectedTowerIndex < 0 || selectedTowerIndex >= towerPrefabs.Count)
            return 0f;

        GameObject prefab = towerPrefabs[selectedTowerIndex];
        if (prefab == null)
            return 0f;

        return TryReadRangeFromPrefab(prefab);
    }

    float TryReadRangeFromPrefab(GameObject prefab)
    {
        MonoBehaviour[] components = prefab.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour comp in components)
        {
            if (comp == null) continue;

            FieldInfo field = comp.GetType().GetField("range", BindingFlags.Public | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(float))
            {
                return (float)field.GetValue(comp);
            }

            PropertyInfo property = comp.GetType().GetProperty("range", BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.PropertyType == typeof(float) && property.CanRead)
            {
                return (float)property.GetValue(comp);
            }
        }

        Debug.LogWarning($"Could not find a public float named 'range' on prefab '{prefab.name}'.");
        return 0f;
    }

    public bool TryGetTowerAtTile(Vector2Int gridPos, out TowerInstance tower)
    {
        return placedTowers.TryGetValue(gridPos, out tower);
    }

    public void RemoveTower(Vector2Int gridPos)
    {
        if (placedTowers.ContainsKey(gridPos))
        {
            placedTowers.Remove(gridPos);
        }
    }
}