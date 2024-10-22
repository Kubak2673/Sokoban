using UnityEngine;
using System.IO;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public GameObject wallPrefab, playerPrefab, boxPrefab, goalPrefab;
    private int currentLevelIndex = 0;
    private string levelsFilePath = "Assets/Levels/levels.txt";
    private string[] allLevels;
    private GameObject playerInstance;
    private LevelUI levelUI;
    private LevelTimer levelTimer;
    private CompletionPopup completionPopup;

    private bool isLevelCompleted = false;

    void Start()
    {
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
        ClearLevel();  // Clear previous level before loading a new one

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
                Vector3 position = new Vector3(x, -y, 0);

                switch (tile)
                {
                    case 'x':
                        Instantiate(wallPrefab, position, Quaternion.identity, transform);
                        break;
                    case 'p':
                        playerInstance = Instantiate(playerPrefab, position, Quaternion.identity, transform);
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
                        break;
                    case 'k':  // Box on goal
                        Instantiate(goalPrefab, position, Quaternion.identity, transform);
                        Instantiate(boxPrefab, position, Quaternion.identity, transform);
                        break;
                }
            }
        }

        isLevelCompleted = false;  // Reset the level completion flag

        levelUI.UpdateLevelText();  // Update level text after loading
        levelTimer.ResetTimer();    // Reset timer after level is loaded
        levelTimer.StartTimer();    // Start the timer after resetting
    }

    void ClearLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);  // Destroy level objects
        }

        if (playerInstance != null)
        {
            Destroy(playerInstance);  // Destroy player instance
            playerInstance = null;
        }

        levelTimer.StopTimer();  // Stop the timer when clearing the level
    }

    public void OnLevelComplete()
    {
        isLevelCompleted = true;  // Mark the level as completed
        levelTimer.StopTimer();  // Stop the timer on level completion
        float time = levelTimer.GetLevelTime();
        completionPopup.ShowPopup(time);  // Show completion popup with level time
    }

    public void OnContinueButton()
    {
        completionPopup.HidePopup();
        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        ClearLevel();  // Clear the current level
        yield return new WaitForSeconds(0.5f);  // Small delay to ensure everything is cleared
        currentLevelIndex++;
        LoadLevel(currentLevelIndex);  // Load the next level
    }

    public bool IsLevelCompleted()
    {
        return isLevelCompleted;  // Return the level completion status
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;  // Return the current level index
    }

    // New method to restart the current level
    public void RestartCurrentLevel()
    {
        ClearLevel(); // Clear current level objects
        levelTimer.ResetTimer(); // Reset the timer
        LoadLevel(currentLevelIndex); // Reload the current level
    }
}
