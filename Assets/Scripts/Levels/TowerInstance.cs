using UnityEngine;

public class TowerInstance : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }

    private TowerPlacementManager placementManager;

    public void Initialize(Vector2Int gridPosition, TowerPlacementManager manager)
    {
        GridPosition = gridPosition;
        placementManager = manager;

        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

    void OnMouseDown()
    {
        Debug.Log($"{name} clicked at tile {GridPosition}");
        // later:
        // placementManager.SelectPlacedTower(this);
    }
}