
using UnityEngine;
using System.IO;
using System.Collections;
using Cinemachine;

public class LevelLoader : MonoBehaviour
{
    public GameObject wallPrefab, playerPrefab, boxPrefab, goalPrefab;
    private int currentLevelIndex = 0;
    private string levelsFilePath = "Assets/Levels/levels.txt";
    private string[] allLevels;
    private CinemachineVirtualCamera virtualCamera;
    private GameObject playerInstance;
    private LevelUI levelUI;
    private LevelTimer levelTimer;
    private CompletionPopup completionPopup;

    private bool isLevelCompleted = false;

    void Start()
    {
        // Find the Cinemachine Virtual Camera in the scene
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("No Cinemachine Virtual Camera found in the scene!");
        }

        levelUI = GetComponent<LevelUI>();
        levelTimer = GetComponent<LevelTimer>();
        completionPopup = GetComponent<CompletionPopup>();

        if (completionPopup == null)
        {
            Debug.LogError("CompletionPopup not found!");
            return;
        }

        LoadLevelsFromFile();
        LoadLevel(currentLevelIndex);
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
                        // Set the Cinemachine Virtual Camera to follow the player (2D)
                        if (virtualCamera != null)
                        {
                            virtualCamera.Follow = playerInstance.transform;
                            virtualCamera.LookAt = null; // In 2D, we don't need LookAt
                        }
                        break;
                    case 'k':  // Box on goal
                        Instantiate(goalPrefab, position, Quaternion.identity, transform);
                        Instantiate(boxPrefab, position, Quaternion.identity, transform);
                        break;
                }
            }
        }

        isLevelCompleted = false;
        levelUI.UpdateLevelText();
        levelTimer.ResetTimer();
        levelTimer.StartTimer();
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

        levelTimer.StopTimer();
    }

    public void OnLevelComplete()
    {
        isLevelCompleted = true;
        levelTimer.StopTimer();
        float time = levelTimer.GetLevelTime();
        completionPopup.ShowPopup(time);
    }

    public void OnContinueButton()
    {
        completionPopup.HidePopup();
        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        ClearLevel();
        yield return new WaitForSeconds(0.5f);
        currentLevelIndex++;
        LoadLevel(currentLevelIndex);
    }

    public void RestartCurrentLevel()
    {
        ClearLevel();
        LoadLevel(currentLevelIndex);
    }

    public bool IsLevelCompleted()
    {
        return isLevelCompleted;
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
}
