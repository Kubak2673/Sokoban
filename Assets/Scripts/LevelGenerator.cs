using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Collections;


public class LevelGenerator : MonoBehaviour
{
    private LevelManager levelManager;
    public Tilemap tilemap; // Reference to the Tilemap
    public RuleTile wallRuleTile; // Reference to the RuleTile for walls
    public RuleTile focalPointRuleTile; // Reference to the RuleTile for the focal point (F)
    public GameObject boxPrefab; // Prefab for the boxes
    public GameObject goalPrefab; // Prefab for the goal
    public Camera mainCamera;
    public static int targetLevelIndex = 0; // Default to level 0
    private Goal goal;
    public int isCompleted = 0;
    private Player player; // Reference to the Player script
    private CompletionPopup completionPopup;
    private StepCounter stepCounter;
    private string[] currentLevel;
    private int currentLevelIndex = 0;
    private string[][] levelsToLoad;
    public static int gems = PlayerPrefs.GetInt("gems", 0);
    private string levelsFilePath;
    public Vector3 focalPoint; // Custom focal point for the camera
    private GameObject playerInstance; // Store player instance to move across levels
    int oldBoxes = 0;

    private async void Start()
    {
        currentLevelIndex = LevelGenerator.targetLevelIndex;
        
        // Use StreamingAssets to load levels from a file
        levelsFilePath = Path.Combine(Application.streamingAssetsPath, "levels.txt");
        
        LoadLevelsFromFile();
        goal = FindObjectOfType<Goal>();
        player = FindObjectOfType<Player>();
        LoadLevel(currentLevelIndex);
    }

    void RemoveOldObjectsExceptPlayer()
    {
        foreach (string tag in new[] { "Box", "Goal" })
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag))
            {
                Destroy(obj);
            }
        }
    }

    void LoadLevelsFromFile()
    {
        if (File.Exists(levelsFilePath))
        {
            string[] allLevels = File.ReadAllText(levelsFilePath).Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);
            levelsToLoad = new string[allLevels.Length][];

            for (int i = 0; i < allLevels.Length; i++)
            {
                levelsToLoad[i] = allLevels[i].Split('\n');
            }
        }
        else
        {
            Debug.LogError("Levels file not found: " + levelsFilePath);
        }
    }

void LoadLevel(int levelIndex)
{
    tilemap.ClearAllTiles();
    oldBoxes = GameObject.FindGameObjectsWithTag("Box").Length;
    RemoveOldObjectsExceptPlayer();
    currentLevel = levelsToLoad[levelIndex];
    PlayerPrefs.SetInt("currentLevel", currentLevelIndex);
    GenerateLevel(currentLevel);
    goal = FindObjectOfType<Goal>();
    Goal.totalBoxes = GameObject.FindGameObjectsWithTag("Box").Length - oldBoxes;
    SetCameraToLevel(currentLevel);
    MovePlayerToStartPosition(currentLevel);
    stepCounter = FindObjectOfType<StepCounter>();
    stepCounter.ResetStepCounter();
    stepCounter.levelText.text = $"Poziom: {currentLevelIndex + 1}";
    
    // Display min steps for the level
    string key = $"minSteps{currentLevelIndex}";
    if (PlayerPrefs.HasKey(key))
    {
        int minSteps = PlayerPrefs.GetInt(key);
        stepCounter.minStepsText.text = $"Rekord:{minSteps}";
    }
    else
    {
        stepCounter.minStepsText.text = "Rekord: --";
    }
}


    void GenerateLevel(string[] level)
    {
        int rows = level.Length;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < level[row].Length; col++)
            {
                Vector3Int tilePosition = new Vector3Int(col, -row, 0);
                char tile = level[row][col];
                switch (tile)
                {
                    case '*':
                        Instantiate(boxPrefab, new Vector3(col, -row, 0), Quaternion.identity);
                        break;
                    case 'X':
                        tilemap.SetTile(tilePosition, wallRuleTile);
                        break;
                    case '.':
                        Instantiate(goalPrefab, new Vector3(col, -row, 0), Quaternion.identity);
                        break;
                    case 'f':
                        tilemap.SetTile(tilePosition, focalPointRuleTile);
                        focalPoint = new Vector3(col, -row, 0);
                        break;
                }
            }
        }
    }

    void SetCameraToLevel(string[] level)
    {
        int levelWidth = level[0].Length;
        int levelHeight = level.Length;
        float cameraHeight = levelHeight + 4;
        float cameraWidth = levelWidth + 4;
        mainCamera.orthographicSize = cameraHeight / 2f;
        Vector3 cameraCenter = new Vector3(levelWidth / 2f, -levelHeight / 2f, mainCamera.transform.position.z);
        mainCamera.transform.position = cameraCenter;
        float minX = 0f;
        float maxX = levelWidth;
        float minY = -levelHeight;
        float maxY = 0f;
        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float cameraHalfHeight = mainCamera.orthographicSize;
        cameraCenter.x = Mathf.Clamp(cameraCenter.x, minX + cameraHalfWidth, maxX - cameraHalfWidth);
        cameraCenter.y = Mathf.Clamp(cameraCenter.y, minY + cameraHalfHeight, maxY - cameraHalfHeight);
        mainCamera.transform.position = cameraCenter;
        MoveCameraToFocalPoint(focalPoint);
    }

    void MoveCameraToFocalPoint(Vector3 focalPoint)
    {
        mainCamera.transform.position = new Vector3(focalPoint.x, focalPoint.y, mainCamera.transform.position.z);
    }

    void MovePlayerToStartPosition(string[] level)
    {
        GameObject player = FindObjectOfType<Player>().gameObject;

        if (player != null)
        {
            for (int row = 0; row < level.Length; row++)
            {
                for (int col = 0; col < level[row].Length; col++)
                {
                    if (level[row][col] == '@')
                    {
                        player.transform.position = new Vector3(col, -row, 0);
                        return; // Player moved, exit the function
                    }
                }
            }
        }
    }

public IEnumerator NextLevel()
{
    if (currentLevelIndex < levelsToLoad.Length - 1)
    {
        IsCompleted();
        yield return new WaitForSeconds(5);

        // Find necessary objects
        completionPopup = FindObjectOfType<CompletionPopup>();
        CompletionPopup.isLevelCompleted = false;
        stepCounter = FindObjectOfType<StepCounter>();

        // Move to the next level
        currentLevelIndex++;
        stepCounter.levelText.text = $"Poziom: {currentLevelIndex + 1}";
        player.ResetUndoHistory();
        goal = FindObjectOfType<Goal>();
        LoadLevel(currentLevelIndex);

        // Reset UI elements
        stepCounter.didIt.text = "";
        stepCounter.didIt.color = Color.white;
        stepCounter.levelText.color = Color.white;

        // Unlock the next level
        if (currentLevelIndex > LevelManager.UnlockedLevels)
        {
            LevelManager.UnlockedLevels = currentLevelIndex;
            PlayerPrefs.SetInt("UnlockedLevels", LevelManager.UnlockedLevels);
            PlayerPrefs.Save(); // Persist the updated value
        }
    }
}



    public void PreviousLevel()
    {
        if (currentLevelIndex > 0)
        {
            completionPopup = FindObjectOfType<CompletionPopup>();
            CompletionPopup.isLevelCompleted = false;
            stepCounter = FindObjectOfType<StepCounter>();
            currentLevelIndex--;
            stepCounter.levelText.text = $"Poziom: {currentLevelIndex + 1}";
            player.ResetUndoHistory();
            goal = FindObjectOfType<Goal>();
            LoadLevel(currentLevelIndex);
            stepCounter.didIt.text = "";
            stepCounter.didIt.color = Color.white;
            stepCounter.levelText.color = Color.white;
        }
    }

    public void RestartLevel()
    {
        player.ResetUndoHistory();
        goal = FindObjectOfType<Goal>();
        LoadLevel(currentLevelIndex);
        stepCounter.didIt.text = "";
        stepCounter.didIt.color = Color.white;
        stepCounter.levelText.color = Color.white;
    }

    public void UpdateFocalPoint(Vector3 newFocalPoint)
    {
        focalPoint = newFocalPoint;
        SetCameraToLevel(currentLevel);
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
    public void IsCompleted()
    {
        if (PlayerPrefs.GetInt("isCompleted" + currentLevelIndex, 0) == 1)
        {
            return;
        }
        PlayerPrefs.SetInt("isCompleted", isCompleted);
        PlayerPrefs.SetInt("currentLevel", currentLevelIndex);
        PlayerPrefs.SetInt("isCompleted" + currentLevelIndex, 1);
        GiveReward();
    }
    private void GiveReward()
    {
        gems++;
        PlayerPrefs.SetInt("gems", gems);
        PlayerPrefs.Save();

        levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.GemsText.text = "Gems: " + gems.ToString();
        }
        else
        {
            Debug.LogError("LevelManager not found in the scene.");
        }
    }
}