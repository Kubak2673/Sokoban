using UnityEngine;
using System.IO;
using System.Collections;
using Cinemachine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Text stepsText; 
    public GameObject wallPrefab, playerPrefab, boxPrefab, goalPrefab, cameraBorderPrefab;
    private int currentLevelIndex = 0;
    private string levelsFilePath = "Assets/Levels/levels.txt";
    private string[] allLevels;
    private CinemachineVirtualCamera virtualCamera;
    private GameObject playerInstance;
    private LevelUI levelUI;
    private CompletionPopup completionPopup;
    public int steps = 0;
    private bool isLevelCompleted = false;
    private Vector3 minBorderPosition, maxBorderPosition; // For camera border limits


    public void LoadLevel3(int levelIndex)
    {
        // Load level logic
        Debug.Log("Loading level: " + levelIndex);
    }
    void Start()
    {
        // Find the Cinemachine Virtual Camera in the scene
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("No Cinemachine Virtual Camera found in the scene!");
        }

        levelUI = GetComponent<LevelUI>();
        completionPopup = GetComponent<CompletionPopup>();

        if (completionPopup == null)
        {
            Debug.LogError("CompletionPopup not found!");
            return;
        }
  // Get the selected level index from PlayerPrefs
            currentLevelIndex = PlayerPrefs.GetInt("SelectedLevelIndex", 0); // Default to 0 if not set
        //  LoadLevel(currentLevelIndex); // Load the level based on the index
        LoadLevelsFromFile();
        LoadLevel(currentLevelIndex);
        //LoadLevel(0);
    }

    void LoadLevelsFromFile()
    {
        if (File.Exists(levelsFilePath))
        {
            allLevels = File.ReadAllText(levelsFilePath).Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);
            Debug.Log("Total levels: " + allLevels.Length);
        }
        else
        {
            Debug.LogError("Levels file not found!");
        }
    }


    void LoadLevel(int levelIndex)
    {
        ClearLevel();

        if (levelIndex >= allLevels.Length)
        {
            Debug.Log("All levels completed!");
            return;
        }

        string[] lines = allLevels[levelIndex].Trim().Split('\n');
        bool cameraBorderInitialized = false;  // Flag to track if the camera border has been initialized

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                char tile = lines[y][x];
                Vector3 position = new Vector3(x, -y, 0); // Keep Z at 0 for 2D

                switch (tile)
                {
                    case 'x':
                        Instantiate(wallPrefab, position, Quaternion.identity, transform);
                        break;
                    case 'p':
                        playerInstance = Instantiate(playerPrefab, position, Quaternion.identity, transform);
                        // Set the Cinemachine Virtual Camera to follow the player (2D)
                        if (virtualCamera != null)
                        {
                            virtualCamera.Follow = playerInstance.transform;
                            virtualCamera.LookAt = null; // In 2D, we don't need LookAt
                        }
                        break;
                    case 'b':
                        Instantiate(boxPrefab, position, Quaternion.identity, transform);
                        break;
                    case 'g':
                        Instantiate(goalPrefab, position, Quaternion.identity, transform);
                        break;
                    case 'l':  // Player on goal
                        Instantiate(goalPrefab, position, Quaternion.identity, transform);
                        playerInstance = Instantiate(playerPrefab, position, Quaternion.identity, transform);
                        if (virtualCamera != null)
                        {
                            virtualCamera.Follow = playerInstance.transform;
                            virtualCamera.LookAt = null;
                        }
                        break;
                    case 'k':  // Box on goal
                        Instantiate(goalPrefab, position, Quaternion.identity, transform);
                        Instantiate(boxPrefab, position, Quaternion.identity, transform);
                        break;
                    case 'r':  // Camera border
                        Instantiate(cameraBorderPrefab, position, Quaternion.identity, transform);
                        // Initialize camera border bounds
                        if (!cameraBorderInitialized)
                        {
                            minBorderPosition = position;
                            maxBorderPosition = position;
                            cameraBorderInitialized = true;
                        }
                        else
                        {
                            // Update min/max positions to expand the bounding area
                            minBorderPosition = Vector3.Min(minBorderPosition, position);
                            maxBorderPosition = Vector3.Max(maxBorderPosition, position);
                        }
                        break;
                }
            }
        }

        // If we found any 'r' tiles, clamp the camera's position
        if (cameraBorderInitialized && virtualCamera != null)
        {
            SetCameraBounds(minBorderPosition, maxBorderPosition);
        }

        isLevelCompleted = false;
        //levelUI.UpdateLevelText(levelIndex + 1);
        // Resetting step count for the new level
        LevelTimer levelTimer = GetComponent<LevelTimer>();
        if (levelTimer != null)
        {
            levelTimer.ResetStepCount(); // Reset the step count when loading a new level
        }
    }

    void SetCameraBounds(Vector3 minPosition, Vector3 maxPosition)
    {
        // Create a confiner for the Cinemachine camera within the bounds
        CinemachineConfiner2D confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        if (confiner != null)
        {
            // Create a 2D bounding box or polygon from the min/max positions
            PolygonCollider2D cameraBounds = new GameObject("CameraBounds").AddComponent<PolygonCollider2D>();
            cameraBounds.transform.position = Vector3.zero;

            // Create vertices for a rectangular boundary
            Vector2[] boundaryPoints = new Vector2[4];
            boundaryPoints[0] = new Vector2(minPosition.x, minPosition.y);  // Bottom-left
            boundaryPoints[1] = new Vector2(maxPosition.x, minPosition.y);  // Bottom-right
            boundaryPoints[2] = new Vector2(maxPosition.x, maxPosition.y);  // Top-right
            boundaryPoints[3] = new Vector2(minPosition.x, maxPosition.y);  // Top-left

            cameraBounds.points = boundaryPoints;
            confiner.m_BoundingShape2D = cameraBounds;
        }
        else
        {
            Debug.LogWarning("No CinemachineConfiner2D found on the virtual camera.");
        }
    }

    void ClearLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (playerInstance != null)
        {
            Destroy(playerInstance);
            playerInstance = null;
        }

        // No need to stop a timer since it's removed
    }

public void OnLevelComplete()
{
    isLevelCompleted = true;
    // Retrieve step count from LevelTimer
    LevelTimer levelTimer = GetComponent<LevelTimer>();
    if (levelTimer != null)
    {
        steps = levelTimer.GetStepCount(); // Get the step count
        completionPopup.ShowPopup(steps); // Show the popup with the step count
    }
}

public void InitializeLevel(int levelIndex)
{
    currentLevelIndex = levelIndex; // Update the current level index
    LoadLevel(currentLevelIndex); // Call the existing LoadLevel method to load the new level
}
    public void OnContinueButton()
    {
        completionPopup.HidePopup();
        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        steps = 0;
        UpdateStepCount(0);
        ClearLevel();
        yield return new WaitForSeconds(0.5f);
        currentLevelIndex++;
        LoadLevel(currentLevelIndex);
    }

    public IEnumerator RestartCurrentLevel()
    {
        // steps = 0;
        // UpdateStepCount(0);
        // ClearLevel();
        //  completionPopup.ShowPopup(steps); // Show the popup with the step count
        // LoadLevel(currentLevelIndex);
        Debug.Log("wywołuję reset");
        steps = 0;
        UpdateStepCount(0);
        ClearLevel();
        yield return new WaitForSeconds(0.5f);
        LoadLevel(currentLevelIndex);
    }

    public bool IsLevelCompleted()
    {
        return isLevelCompleted;
    }
public void UpdateStepCount(int steps)
{
    // Handle the step count here if needed
    stepsText.text = $" s:{steps}"; // Update steps text
    // Debug.Log("Current step count: " + steps);
}

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
}
