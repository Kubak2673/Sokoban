using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Make sure this is included for Gamepad support

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Reference to the pause menu UI
    private bool isPaused = false;
    private LevelLoader levelLoader;

    void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>(); // Find LevelLoader in the scene
        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader not found in the scene!");
        }
    }

    void Update()
    {
        // Check for the Escape key or Gamepad button to toggle the pause menu
        if (Input.GetKeyDown(KeyCode.Escape) || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume game time
        isPaused = false; // Update pause state
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Pause game time
        isPaused = true; // Update pause state
    }

    public void RestartLevel()
    {

        if (levelLoader != null)
        {
                    Debug.Log($"!!!levelLoader: {levelLoader}");
            StartCoroutine(levelLoader.RestartCurrentLevel()); // Start the coroutine
        }
    }

    public void QuitGame()
    {
        Application.Quit(); // Quit the application
        Debug.Log("Quit Game"); // Log to console
    }
}
