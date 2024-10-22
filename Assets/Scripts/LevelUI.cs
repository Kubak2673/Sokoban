using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // This is necessary to access the Gamepad class.

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
        UpdateLevelText();
    }

    public void UpdateLevelText()
    {
        if (levelLoader != null) // Ensure levelLoader is not null
        {
            int currentLevel = levelLoader.GetCurrentLevelIndex() + 1;
            levelText.text = " " + currentLevel;
        }
        else
        {
            Debug.LogError("LevelLoader reference is null.");
        }
    }
}
