using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSequence : MonoBehaviour
{
    public static int currentLevelIndex = 0;

    public MapLoader mapLoader;
    public TextAsset[] levels;

    void Awake()
    {
        if (mapLoader == null)
        {
            mapLoader = FindAnyObjectByType<MapLoader>();
        }

        if (mapLoader == null)
        {
            Debug.LogWarning("LevelSequence could not find a MapLoader.");
            return;
        }

        if (levels == null || levels.Length == 0)
        {
            Debug.LogWarning("LevelSequence has no levels assigned.");
            return;
        }

        if (currentLevelIndex < 0 || currentLevelIndex >= levels.Length)
        {
            StartCoroutine(ExitAfterDelay());
        }

        mapLoader.mapFile = levels[currentLevelIndex];
    }

    public IEnumerator ExitAfterDelay()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("MainMenu");
    }

    public static void ResetToFirstLevel()
    {
        currentLevelIndex = 0;
    }
}