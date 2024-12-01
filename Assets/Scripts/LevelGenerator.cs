using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    public Tilemap tilemap;               // Reference to the Tilemap
    public RuleTile wallRuleTile;         // Reference to the RuleTile for walls
    public GameObject playerPrefab;       // Prefab for the player
    public GameObject boxPrefab;          // Prefab for the boxes
    public GameObject goalPrefab;         // Prefab for the goal
    public Camera mainCamera;             // Reference to the main camera

    // Level layout represented as a multiline string array
    public string[] level1 = new string[] {
        "  XXXXXXXX   ",          // Row 1: Walls
        "   X  ....X   ",          // Row 2: Walls and Empty spaces
        "   XXXXXXXXXXXX  ....X",  // Row 3: Walls and Empty spaces
        "   X    X  * *   ....X",  // Row 4: Walls, Player, Box, and Empty spaces
        "   X *** X  ....X",  // Row 5: Walls, Boxes, and Empty spaces
        "   X  *     * X  ....X",  // Row 6: Walls, Boxes, and Empty spaces
        "   X ** XXX",  // Row 7: Walls and Boxes
        "   XXXX  * X     X",      // Row 8: Walls and Boxes
        "   X   X XXXXXXXXX",      // Row 9: Walls and Boxes
        "   X    *  XX",           // Row 10: Box and Wall
        "   X **X** @X",           // Row 11: Box, Player, and Walls
        "   X   X   XX",           // Row 12: Wall
        "  XXXXXXXXX  "             // Row 13: Wall
    };
    public string[] level2 = new string[] {
        "k  XXXXXXXX   k",          // Row 1: Walls
        "   X  ....X   ",          // Row 2: Walls and Empty spaces
        "   XXXXXXXXXXXX  ....X",  // Row 3: Walls and Empty spaces
        "   X    X  * *   ....X",  // Row 4: Walls, Player, Box, and Empty spaces
        "   X *** X  ....X",  // Row 5: Walls, Boxes, and Empty spaces
        "   X  *     * X  ....X",  // Row 6: Walls, Boxes, and Empty spaces
        "   X ** XXX",  // Row 7: Walls and Boxes
        "   XXXX  * X     X",      // Row 8: Walls and Boxes
        "   X   X XXXXXXXXX",      // Row 9: Walls and Boxes
        "   X    *  XX",           // Row 10: Box and Wall
        "   X **X** @X",           // Row 11: Box, Player, and Walls
        "   X   X   XX",           // Row 12: Wall
        "k  XXXXXXXXX  k"             // Row 13: Wall
    };

    public string[] level3 = new string[] {
        "k  XXXXXXXX   k",          // Row 1: Walls
        "   X  ....X   ",          // Row 2: Walls and Empty spaces
        "   XXXXXXXXXXXX  ....X",  // Row 3: Walls and Empty spaces
        "   X    X  * *   ....X",  // Row 4: Walls, Player, Box, and Empty spaces
        "   X *** X  ....X",  // Row 5: Walls, Boxes, and Empty spaces
        "   X  *     * X  ....X",  // Row 6: Walls, Boxes, and Empty spaces
        "   X ** XXX",  // Row 7: Walls and Boxes
        "   XXXX  * X     X",      // Row 8: Walls and Boxes
        "   X   X XXXXXXXXX",      // Row 9: Walls and Boxes
        "   X    *  XX",           // Row 10: Box and Wall
        "   X **X** @X",           // Row 11: Box, Player, and Walls
        "   X   X   XX",           // Row 12: Wall
        "k  XXXXXXXXX  k"             // Row 13: Wall
    };

    private string[] currentLevel;
    private int currentLevelIndex = 0;

    // Array of levels to load (multiple levels)
    private string[][] levelsToLoad;

    void Start()
    {
        // Initialize levels
        levelsToLoad = new string[][] { level1, level2, level3 };

        // Load the first level
        LoadLevel(currentLevelIndex);
    }

    void LoadLevel(int levelIndex)
    {
        // Clear the current Tilemap and remove all previously instantiated objects
        tilemap.ClearAllTiles();

        // Remove all previous level objects (player, boxes, goals)
        RemoveOldObjects();

        // Set the current level
        currentLevel = levelsToLoad[levelIndex];

        // Generate the new level
        GenerateLevel(currentLevel);

        // Set camera bounds based on the level's dimensions and k tiles
        SetCameraBounds(currentLevel);
    }

    void RemoveOldObjects()
    {
        // Find and destroy any game objects with specific tags or names that are part of the previous level
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Optionally, destroy any specific objects (like the player or goals) if needed
        GameObject[] objectsToRemove = GameObject.FindGameObjectsWithTag("Player");
        foreach (var obj in objectsToRemove)
        {
            Destroy(obj);
        }

        objectsToRemove = GameObject.FindGameObjectsWithTag("Box");
        foreach (var obj in objectsToRemove)
        {
            Destroy(obj);
        }

        objectsToRemove = GameObject.FindGameObjectsWithTag("Goal");
        foreach (var obj in objectsToRemove)
        {
            Destroy(obj);
        }
    }

    void GenerateLevel(string[] level)
    {
        int rows = level.Length;
        int cols = GetMaxRowLength(level);

        // Loop through the array and place tiles on the Tilemap
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < level[row].Length; col++)
            {
                Vector3Int tilePosition = new Vector3Int(col, -row, 0); // Adjust y for proper orientation
                char tile = level[row][col];

                switch (tile)
                {
                    case '@': // Player
                        Instantiate(playerPrefab, new Vector3(col, -row, 0), Quaternion.identity);
                        break;
                    case '*': // Box
                        Instantiate(boxPrefab, new Vector3(col, -row, 0), Quaternion.identity);
                        break;
                    case 'X': // Wall (RuleTile)
                        tilemap.SetTile(tilePosition, wallRuleTile);
                        break;
                    case '.': // Goal (if needed, you can create a goal prefab too)
                        Instantiate(goalPrefab, new Vector3(col, -row, 0), Quaternion.identity);
                        break;
                    case 'k': // Camera Bound Tile
                        // You can add custom behavior for camera bounds if needed.
                        break;
                    case ' ': // Empty space (nothing needed here)
                        break;
                }
            }
        }
    }

    // Set the camera bounds based on the level size and the 'k' tiles
    void SetCameraBounds(string[] level)
    {
        int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;

        // Loop through the level to find the k tiles and get their bounds
        for (int row = 0; row < level.Length; row++)
        {
            for (int col = 0; col < level[row].Length; col++)
            {
                if (level[row][col] == 'k')
                {
                    minX = Mathf.Min(minX, col);
                    maxX = Mathf.Max(maxX, col);
                    minY = Mathf.Min(minY, row);
                    maxY = Mathf.Max(maxY, row);
                }
            }
        }

        // Check if any 'k' tiles were found
        if (minX != int.MaxValue && maxX != int.MinValue && minY != int.MaxValue && maxY != int.MinValue)
        {
            // Set camera bounds to the area containing the 'k' tiles
            Vector3 cameraPosition = new Vector3((minX + maxX) / 2f, -(minY + maxY) / 2f, mainCamera.transform.position.z);
            mainCamera.transform.position = cameraPosition;

            // Adjust the orthographic size to fit the camera bounds
            float cameraWidth = maxX - minX + 1;
            float cameraHeight = maxY - minY + 1;
            mainCamera.orthographicSize = Mathf.Max(cameraWidth, cameraHeight) / 2f;
        }
        else
        {
            // Default camera position and size if no 'k' tiles are found
            mainCamera.orthographicSize = Mathf.Max(level.Length, GetMaxRowLength(level)) / 2f;
            mainCamera.transform.position = new Vector3(GetMaxRowLength(level) / 2f, -level.Length / 2f, mainCamera.transform.position.z);
        }
    }

    // Get the maximum length of any row in the level (to ensure camera bounds are calculated correctly)
    int GetMaxRowLength(string[] level)
    {
        int maxLength = 0;

        foreach (string row in level)
        {
            maxLength = Mathf.Max(maxLength, row.Length);
        }

        return maxLength;
    }

    // Load next level
    public void NextLevel()
    {
        if (currentLevelIndex < levelsToLoad.Length - 1)
        {
            currentLevelIndex++;
            LoadLevel(currentLevelIndex);
        }
        else
        {
            Debug.Log("You are on the last level.");
        }
    }

    // Load previous level
    public void PreviousLevel()
    {
        if (currentLevelIndex > 0)
        {
            currentLevelIndex--;
            LoadLevel(currentLevelIndex);
        }
        else
        {
            Debug.Log("You are on the first level.");
        }
    }

    // Restart the current level
    public void RestartLevel()
    {
        LoadLevel(currentLevelIndex);
    }
}
