using UnityEngine;

public class TowerInstance : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }

    private TowerInspector towerInspector;

    public void Initialize(Vector2Int gridPosition, TowerInspector inspector)
    {
        GridPosition = gridPosition;
        towerInspector = inspector;

        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

    void OnMouseDown()
    {
        towerInspector.SelectTower(this.gameObject.GetComponent<Tower>());
    }
}