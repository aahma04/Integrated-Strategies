using UnityEngine;

public class EndTile : MonoBehaviour
{
    public int lives = 3;


    public void TakeDamage(int amount)
    {
        lives -= amount;

        if (lives <= 0)
        {
            lives = 0;

            EndTile[] allEndTiles = FindObjectsOfType<EndTile>();

            foreach (EndTile tile in allEndTiles)
            {
                tile.lives = lives;
            }

            MapLoader mapLoader = FindObjectOfType<MapLoader>();
            if (mapLoader != null)
            {
                mapLoader.GameOver();
            }
        }
    }
}
