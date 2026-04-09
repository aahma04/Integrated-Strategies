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

    [Header("Tower Prefabs")]
    public List<TowerPrefabEntry> towerPrefabs = new();

    [Header("Preview")]
    public Color validPreviewColor = new Color(0f, 1f, 0f, 0.45f);
    public Color invalidPreviewColor = new Color(1f, 0f, 0f, 0.45f);
    public Color validRangeColor = new Color(0f, 0.7f, 1f, 0.18f);
    public Color invalidRangeColor = new Color(1f, 0.2f, 0.2f, 0.18f);

    private int selectedTowerIndex = -1;
    private GameObject previewObject;
    private RangeCirclePreview rangePreview;

    private readonly Dictionary<Vector2Int, TowerInstance> placedTowers = new();

    void Start()
    {
        CreateRangePreview();
    }

    void Update()
    {
        if (mainCamera == null || mapLoader == null)
            return;

        if (selectedTowerIndex < 0 || selectedTowerIndex >= towerPrefabs.Count)
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

        bool isValid = IsValidPlacement(gridPos);

        ShowPreviewAt(snappedPos, isValid);
        ShowRangeAt(snappedPos, GetSelectedTowerRange(), isValid);

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceSelectedTower(gridPos, snappedPos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClearSelection();
        }
    }

    public void SelectTowerByIndex(int index)
    {
        if (index < 0 || index >= towerPrefabs.Count)
        {
            Debug.LogWarning($"Invalid tower index: {index}");
            return;
        }

        selectedTowerIndex = index;
        RebuildPreviewObject();
    }

    public void ClearSelection()
    {
        selectedTowerIndex = -1;

        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        HidePreviews();
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

        GameObject towerPrefab = towerPrefabs[selectedTowerIndex].prefab;

        if (towerPrefab == null)
        {
            Debug.LogWarning("Selected tower prefab is null.");
            return;
        }

        GameObject placedTower = Instantiate(towerPrefab, snappedPos, Quaternion.identity);

        TowerInstance towerInstance = placedTower.GetComponent<TowerInstance>();
        if (towerInstance == null)
        {
            towerInstance = placedTower.AddComponent<TowerInstance>();
        }

        towerInstance.Initialize(gridPos, this);
        placedTowers[gridPos] = towerInstance;
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
            RebuildPreviewObject();
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

    void RebuildPreviewObject()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        if (selectedTowerIndex < 0 || selectedTowerIndex >= towerPrefabs.Count)
            return;

        GameObject prefab = towerPrefabs[selectedTowerIndex].prefab;
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

        GameObject prefab = towerPrefabs[selectedTowerIndex].prefab;
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
}