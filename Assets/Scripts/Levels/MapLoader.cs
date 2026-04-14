using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class EnemyPrefabEntry
{
    public string enemyName;
    public GameObject prefab;
}

public enum SpawnInstructionType
{
    Enemy,
    Break
}

[System.Serializable]
public class SpawnInstruction
{
    public SpawnInstructionType type;
    public string enemyName;
    public float delay;
}

public class MapLoader : MonoBehaviour
{
    [Header("Map File")]
    public TextAsset mapFile;

    [Header("Prefabs")]
    public GameObject grassPrefab;
    public GameObject pathPrefab;
    public GameObject startPrefab;
    public GameObject endPrefab;

    [Header("Enemy Prefabs")]
    public List<EnemyPrefabEntry> enemyPrefabs = new();

    [Header("Spawn Settings")]
    public float timeBetweenSpawns = 1f;
    public bool autoStartSpawning = false;

    public float tileSize = 1f;

    [Header("Camera")]
    public Camera mainCamera;
    public float cameraPadding = 1f;

    [Header("Various Texts")]
    public GameObject gameOverText;
    public GameObject pressP;

    [Header("Tower Inspector")]
    public TowerInspector towerInspector;

    private Dictionary<char, char> startToEnd = new();
    private Dictionary<char, List<SpawnInstruction>> spawnInstructionsByStart = new();

    private Dictionary<char, Vector2Int> startPositions = new();
    private Dictionary<char, Vector2Int> endPositions = new();
    private Dictionary<char, EndTile> endTileInstances = new();

    private Dictionary<char, List<Vector3>> pathsByStart = new();
    private Dictionary<string, GameObject> enemyPrefabLookup = new();

    private int activeEnemyCount = 0;
    private int activeSpawnerCount = 0;

    private bool levelStarted;
    

    private char[,] grid;

    void Start()
    {
        BuildEnemyPrefabLookup();
        LoadMap();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && levelStarted == false)
        {
            StartAllSpawns();
            pressP.SetActive(false);
            levelStarted = true;
        }
    }

    void BuildEnemyPrefabLookup()
    {
        enemyPrefabLookup.Clear();

        foreach (EnemyPrefabEntry entry in enemyPrefabs)
        {
            if (entry == null) continue;
            if (string.IsNullOrWhiteSpace(entry.enemyName)) continue;
            if (entry.prefab == null) continue;

            string key = entry.enemyName.Trim().ToLower();

            if (enemyPrefabLookup.ContainsKey(key))
            {
                Debug.LogWarning($"Duplicate enemy name '{entry.enemyName}' found. Overwriting previous prefab.");
            }

            enemyPrefabLookup[key] = entry.prefab;
        }
    }

    void LoadMap()
    {
        if (mapFile == null)
        {
            Debug.LogError("No map file assigned.");
            return;
        }

        startToEnd.Clear();
        spawnInstructionsByStart.Clear();
        startPositions.Clear();
        endPositions.Clear();
        pathsByStart.Clear();

        string[] lines = mapFile.text.Split('\n');

        string section = "";
        List<string> gridLines = new();

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (section == "[grid]" && string.IsNullOrEmpty(line))
            {
                break;
            }

            if (string.IsNullOrEmpty(line)) continue;
            if (line.StartsWith("//")) continue;
            if (line.StartsWith("#")) continue;

            if (line.StartsWith("["))
            {
                section = line;
                continue;
            }

            switch (section)
            {
                case "[start_end_pairs]":
                    ParseStartEnd(line);
                    break;

                case "[spawns]":
                    ParseSpawns(line);
                    break;

                case "[grid]":
                    gridLines.Add(line);
                    break;
            }
        }

        if (gridLines.Count == 0)
        {
            Debug.LogError("No grid lines were found in the map file.");
            return;
        }

        BuildGrid(gridLines);
        GenerateAllPaths();
        PositionCamera();
        SpawnTiles();
    }

    void ParseStartEnd(string line)
    {
        string[] parts = line.Split('=');

        if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
        {
            Debug.LogError("Invalid start_end_pairs line: " + line);
            return;
        }

        string starts = parts[0].Trim();
        char end = parts[1].Trim()[0];

        foreach (char start in starts)
        {
            startToEnd[start] = end;
        }
    }

    void ParseSpawns(string line)
    {
        // Example:
        // S:normal:3,fast:2,break:4,tank:1

        string[] firstSplit = line.Split(':', 2);

        if (firstSplit.Length != 2 || string.IsNullOrWhiteSpace(firstSplit[0]) || string.IsNullOrWhiteSpace(firstSplit[1]))
        {
            Debug.LogError("Invalid spawns line: " + line);
            return;
        }

        char start = firstSplit[0].Trim()[0];
        string sequenceText = firstSplit[1].Trim();

        string[] entries = sequenceText.Split(',');
        List<SpawnInstruction> instructions = new();

        foreach (string rawEntry in entries)
        {
            string entry = rawEntry.Trim();
            if (string.IsNullOrEmpty(entry)) continue;

            string[] parts = entry.Split(':');

            if (parts.Length != 2)
            {
                Debug.LogError("Invalid spawn entry '" + entry + "' in line: " + line);
                continue;
            }

            string name = parts[0].Trim().ToLower();
            string amountText = parts[1].Trim();

            if (!int.TryParse(amountText, out int amount) || amount < 0)
            {
                Debug.LogError("Invalid amount in spawn entry '" + entry + "' in line: " + line);
                continue;
            }

            if (name == "break")
            {
                instructions.Add(new SpawnInstruction
                {
                    type = SpawnInstructionType.Break,
                    delay = amount
                });
            }
            else
            {
                if (!enemyPrefabLookup.ContainsKey(name))
                {
                    Debug.LogError($"Enemy name '{name}' in line '{line}' does not match any enemy prefab entry.");
                    continue;
                }

                for (int i = 0; i < amount; i++)
                {
                    instructions.Add(new SpawnInstruction
                    {
                        type = SpawnInstructionType.Enemy,
                        enemyName = name
                    });
                }
            }
        }

        spawnInstructionsByStart[start] = instructions;
    }

    void BuildGrid(List<string> gridLines)
    {
        int height = gridLines.Count;
        int width = gridLines[0].Length;

        for (int i = 1; i < gridLines.Count; i++)
        {
            if (gridLines[i].Length != width)
            {
                Debug.LogError("Grid rows are not all the same length.");
                return;
            }
        }

        grid = new char[width, height];
        startPositions.Clear();
        endPositions.Clear();

        for (int y = 0; y < height; y++)
        {
            string line = gridLines[height - 1 - y];

            for (int x = 0; x < width; x++)
            {
                char c = line[x];
                grid[x, y] = c;

                if (char.IsUpper(c))
                {
                    if (startToEnd.ContainsKey(c))
                    {
                        startPositions[c] = new Vector2Int(x, y);
                    }
                    else
                    {
                        endPositions[c] = new Vector2Int(x, y);
                    }
                }
            }
        }
    }

    void SpawnTiles()
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        endTileInstances.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                char tile = grid[x, y];
                Vector3 pos = GridToWorld(new Vector2Int(x, y));

                GameObject prefab = GetPrefab(tile);

                if (prefab != null)
                {
                    GameObject spawned = Instantiate(prefab, pos, Quaternion.identity, transform);

                    if (char.IsUpper(tile) && !startToEnd.ContainsKey(tile))
                    {
                        EndTile endTile = spawned.GetComponent<EndTile>();

                        if (endTile != null)
                        {
                            endTileInstances[tile] = endTile;
                        }
                        else
                        {
                            Debug.LogWarning($"End tile prefab at '{tile}' is missing an EndTile script.");
                        }
                    }
                }
            }
        }
    }

    GameObject GetPrefab(char tile)
    {
        switch (tile)
        {
            case '.':
                return grassPrefab;

            case '#':
                return pathPrefab;

            default:
                if (char.IsUpper(tile))
                {
                    if (startToEnd.ContainsKey(tile))
                        return startPrefab;
                    else
                        return endPrefab;
                }

                return grassPrefab;
        }
    }

    void PositionCamera()
{
    if (mainCamera == null)
    {
        Debug.LogWarning("Main camera not assigned.");
        return;
    }

    int width = grid.GetLength(0);
    int height = grid.GetLength(1);

    float worldWidth = width * tileSize;
    float worldHeight = height * tileSize;

    float reservedRightPercent = 0.25f;
    float reservedBottomPercent = 0.25f;

    float usableWidthPercent = 1f - reservedRightPercent;   // 0.75
    float usableHeightPercent = 1f - reservedBottomPercent; // 0.75

    float fullAspect = (float)Screen.width / Screen.height;

    float usableAspect = (Screen.width * usableWidthPercent) / (Screen.height * usableHeightPercent);

    float verticalSize = worldHeight / 2f + cameraPadding;
    float horizontalSize = (worldWidth / usableAspect) / 2f + cameraPadding;

    mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);

    float visibleWorldHeight = mainCamera.orthographicSize * 2f;
    float visibleWorldWidth = visibleWorldHeight * fullAspect;

    // Usable area center in normalized screen space:
    // x goes from 0 to 0.75, so center is 0.375
    // y goes from 0.25 to 1.0, so center is 0.625
    float usableCenterXNormalized = usableWidthPercent / 2f;
    float usableCenterYNormalized = reservedBottomPercent + usableHeightPercent / 2f;

    float usableCenterOffsetX = (usableCenterXNormalized - 0.5f) * visibleWorldWidth;
    float usableCenterOffsetY = (usableCenterYNormalized - 0.5f) * visibleWorldHeight;

    float mapCenterX = (worldWidth - tileSize) / 2f;
    float mapCenterY = (worldHeight - tileSize) / 2f;

    mainCamera.transform.position = new Vector3(
        mapCenterX - usableCenterOffsetX,
        mapCenterY - usableCenterOffsetY,
        mainCamera.transform.position.z
    );
}

    void GenerateAllPaths()
    {
        pathsByStart.Clear();

        foreach (KeyValuePair<char, char> pair in startToEnd)
        {
            char startSymbol = pair.Key;
            char endSymbol = pair.Value;

            if (!startPositions.ContainsKey(startSymbol))
            {
                Debug.LogError($"Start '{startSymbol}' not found in grid.");
                continue;
            }

            if (!endPositions.ContainsKey(endSymbol))
            {
                Debug.LogError($"End '{endSymbol}' not found in grid.");
                continue;
            }

            Vector2Int startPos = startPositions[startSymbol];
            Vector2Int endPos = endPositions[endSymbol];

            List<Vector2Int> gridPath = FindPathBFS(startPos, endPos);

            if (gridPath == null)
            {
                Debug.LogError($"No valid path found from '{startSymbol}' to '{endSymbol}'.");
                continue;
            }

            List<Vector3> worldPath = new();

            foreach (Vector2Int cell in gridPath)
            {
                worldPath.Add(GridToWorld(cell));
            }

            pathsByStart[startSymbol] = worldPath;

            Debug.Log($"Path built for {startSymbol} -> {endSymbol}, length = {worldPath.Count}");
        }
    }

    List<Vector2Int> FindPathBFS(Vector2Int start, Vector2Int end)
    {
        Queue<Vector2Int> queue = new();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new();
        HashSet<Vector2Int> visited = new();

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == end)
            {
                return ReconstructPath(cameFrom, start, end);
            }

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (!IsInBounds(next)) continue;
                if (visited.Contains(next)) continue;
                if (!IsWalkable(next, end)) continue;

                visited.Add(next);
                queue.Enqueue(next);
                cameFrom[next] = current;
            }
        }

        return null;
    }

    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new();
        Vector2Int current = end;

        path.Add(current);

        while (current != start)
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 &&
               pos.y >= 0 &&
               pos.x < grid.GetLength(0) &&
               pos.y < grid.GetLength(1);
    }

    bool IsWalkable(Vector2Int pos, Vector2Int end)
    {
        if (pos == end) return true;

        char tile = grid[pos.x, pos.y];
        return tile == '#';
    }

    Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, -2f);
    }

    public List<Vector3> GetPathForStart(char startSymbol)
    {
        if (pathsByStart.TryGetValue(startSymbol, out List<Vector3> path))
        {
            return path;
        }

        return null;
    }

    public Vector3 GetWorldPositionForStart(char startSymbol)
    {
        if (startPositions.TryGetValue(startSymbol, out Vector2Int pos))
        {
            return GridToWorld(pos);
        }

        return Vector3.zero;
    }

    public List<SpawnInstruction> GetSpawnInstructionsForStart(char startSymbol)
    {
        if (spawnInstructionsByStart.TryGetValue(startSymbol, out List<SpawnInstruction> instructions))
        {
            return instructions;
        }

        return null;
    }

    public GameObject GetEnemyPrefabByName(string enemyName)
    {
        if (string.IsNullOrWhiteSpace(enemyName))
        {
            return null;
        }

        enemyPrefabLookup.TryGetValue(enemyName.Trim().ToLower(), out GameObject prefab);
        return prefab;
    }

    public void StartAllSpawns()
    {
        activeSpawnerCount = spawnInstructionsByStart.Count;

        foreach (char startSymbol in spawnInstructionsByStart.Keys)
        {
            StartCoroutine(SpawnFromStart(startSymbol));
        }
    }

    public IEnumerator SpawnFromStart(char startSymbol)
    {
        List<SpawnInstruction> instructions = GetSpawnInstructionsForStart(startSymbol);

        if (instructions == null || instructions.Count == 0)
        {
            Debug.LogWarning($"No spawn instructions found for start '{startSymbol}'.");
            yield break;
        }

        Vector3 spawnPos = GetWorldPositionForStart(startSymbol);
        List<Vector3> path = GetPathForStart(startSymbol);

        if (path == null || path.Count == 0)
        {
            Debug.LogError($"No path found for start '{startSymbol}'.");
            yield break;
        }

        foreach (SpawnInstruction instruction in instructions)
        {
            if (instruction.type == SpawnInstructionType.Break)
            {
                yield return new WaitForSeconds(instruction.delay);
            }
            else if (instruction.type == SpawnInstructionType.Enemy)
            {
                GameObject prefab = GetEnemyPrefabByName(instruction.enemyName);

                if (prefab == null)
                {
                    Debug.LogError($"No prefab found for enemy '{instruction.enemyName}'.");
                    continue;
                }

                GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
                NotifyEnemySpawned();

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.SetPath(path);
                    enemyScript.SetMapLoader(this);
                    enemyScript.SetTargetEnd(startToEnd[startSymbol]);
                }
                else
                {
                    Debug.LogError($"Spawned prefab '{prefab.name}' does not have an Enemy script attached.");
                }

                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        activeSpawnerCount--;
        CheckLevelComplete();
    }

    void OnDrawGizmos()
    {
        if (pathsByStart == null) return;

        Gizmos.color = Color.yellow;

        foreach (var pair in pathsByStart)
        {
            List<Vector3> path = pair.Value;

            if (path == null || path.Count == 0) continue;

            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.DrawSphere(path[i], 0.1f);

                if (i < path.Count - 1)
                {
                    Gizmos.DrawLine(path[i], path[i + 1]);
                }
            }
        }
    }

    public void NotifyEnemySpawned()
    {
        activeEnemyCount++;
    }

    public void NotifyEnemyRemoved()
    {
        activeEnemyCount--;

        if (activeEnemyCount < 0)
        {
            activeEnemyCount = 0;
        }

        CheckLevelComplete();
    }

    void CheckLevelComplete()
    {
        if (activeSpawnerCount == 0 && activeEnemyCount == 0)
        {
            Debug.Log("Level complete.");
            AdvanceToNextLevel();
        }
    }

    void AdvanceToNextLevel()
    {
        LevelSequence.currentLevelIndex++;

        LevelSequence levelSequence = FindAnyObjectByType<LevelSequence>();

        if (levelSequence != null && LevelSequence.currentLevelIndex >= levelSequence.levels.Length)
        {
            Debug.Log("All levels complete.");
            StartCoroutine(ExitAfterDelay());
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void DamageEndTile(char endSymbol, int amount)
    {
        if (!endTileInstances.TryGetValue(endSymbol, out EndTile endTile) || endTile == null)
        {
            Debug.LogWarning($"No EndTile instance found for '{endSymbol}'.");
            return;
        }

        endTile.lives -= amount;

        if (endTile.lives <= 0)
        {
            endTile.lives = 0;
            GameOver();
        }
    }

    public IEnumerator ExitAfterDelay()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Mainmenu");
    }

    public void GameOver()
    {

        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
            LevelSequence.currentLevelIndex = 0;
            StartCoroutine(ExitAfterDelay());
        }

        enabled = false;
    }

    //HELPER FUNCTIONS SO OTHER FILES CAN ACCESS POSITIONING/TILE INFO
    public bool TryGetGridPositionFromWorld(Vector3 worldPos, out Vector2Int gridPos)
    {
        gridPos = new Vector2Int(
            Mathf.RoundToInt(worldPos.x / tileSize),
            Mathf.RoundToInt(worldPos.y / tileSize)
        );

        return IsInBounds(gridPos);
    }

    public bool IsGrassTile(Vector2Int gridPos)
    {
        if (!IsInBounds(gridPos)) return false;
        return grid[gridPos.x, gridPos.y] == '.';
    }

    public bool IsPathTile(Vector2Int gridPos)
    {
        if (!IsInBounds(gridPos)) return false;
        return grid[gridPos.x, gridPos.y] == '#';
    }

    public bool IsStartOrEndTile(Vector2Int gridPos)
    {
        if (!IsInBounds(gridPos)) return false;
        return char.IsUpper(grid[gridPos.x, gridPos.y]);
    }

    public bool IsPlaceableTile(Vector2Int gridPos)
    {
        if (!IsInBounds(gridPos)) return false;
        return IsGrassTile(gridPos);
    }

    public Vector3 GetSnappedWorldPosition(Vector2Int gridPos)
    {
        return GridToWorld(gridPos);
    }

    public bool IsValidGridPosition(Vector2Int gridPos)
    {
        return IsInBounds(gridPos);
    }

    void OnMouseDown()
    {
        towerInspector.SelectTower(null);
    }
}