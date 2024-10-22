using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button level1Button; // Reference to the Level 1 button
    public Button level2Button; // Reference to the Level 2 button
    public Button level3Button; // Reference to the Level 3 button
    public GameObject levelSelectionPanel; // Panel that contains the level buttons

    void Start()
    {
        // Add listeners to level buttons
        level1Button.onClick.AddListener(() => StartGame(0)); // Level 1
        level2Button.onClick.AddListener(() => StartGame(1)); // Level 2
        level3Button.onClick.AddListener(() => StartGame(2)); // Level 3
        // Add more listeners for additional levels if necessary
    }

    void StartGame(int levelIndex)
    {
        // Store the selected level index and load the game scene
        PlayerPrefs.SetInt("SelectedLevel", levelIndex);
        SceneManager.LoadScene(1); // Assuming the game scene is at index 1
    }
}
