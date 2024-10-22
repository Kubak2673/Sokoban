using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public Text levelText;  // Reference to the UI Text component
    private LevelLoader levelLoader;

    void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        
        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader not found in the scene!");
            return; // Exit Start if LevelLoader is not found
        }

        if (levelText == null) // Ensure levelText is assigned
        {
            Debug.LogError("LevelText is not assigned in the inspector!");
            return; // Exit Start if levelText is null
        }

        UpdateLevelText();
    }

    public void UpdateLevelText()
    {
        if (levelLoader != null) // Ensure levelLoader is not null
        {
            int currentLevel = levelLoader.GetCurrentLevelIndex() + 1;
            levelText.text = "Level: " + currentLevel;
        }
        else
        {
            Debug.LogError("LevelLoader reference is null.");
        }
    }
}
