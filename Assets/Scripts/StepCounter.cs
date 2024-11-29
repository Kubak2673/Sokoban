using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Required for the new Input System

public class StepCounter : MonoBehaviour
{
    public Text stepCounterText;
    public Text levelText;
    private int stepCount = 0;
    public Button nextLevelButton;
    public Button previousLevelButton;
    public Button restartButton;

    private void Start()
    {
        UpdateStepCounter();
        UpdateLevelText();

        // Add listeners for button clicks
        nextLevelButton.onClick.AddListener(NextLevel);
        previousLevelButton.onClick.AddListener(PreviousLevel);
        restartButton.onClick.AddListener(RestartLevel);
    }

    private void Update()
    {
        // Check for Gamepad inputs
        if (Gamepad.current != null)
        {
            // Right Bumper (RB) for "Next Level"
            if (Gamepad.current.rightShoulder.wasPressedThisFrame)
            {
                NextLevel();
                return;
            }

            // Left Bumper (LB) for "Previous Level"
            if (Gamepad.current.leftShoulder.wasPressedThisFrame)
            {
                PreviousLevel();
                return;
            }

            // Y Button for "Restart Level"
            if (Gamepad.current.yButton.wasPressedThisFrame)
            {
                RestartLevel();
                return;
            }
        }

        // Check for Keyboard inputs
        if (Input.GetKeyDown(KeyCode.E)) // E key for "Next Level"
        {
            NextLevel();
        }

        if (Input.GetKeyDown(KeyCode.Q)) // Q key for "Previous Level"
        {
            PreviousLevel();
        }

        if (Input.GetKeyDown(KeyCode.R)) // R key for "Restart Level"
        {
            RestartLevel();
        }
    }

    public void IncrementStepCounter()
    {
        stepCount++;
        UpdateStepCounter();
    }

    private void UpdateStepCounter()
    {
        if (stepCounterText != null)
            stepCounterText.text = $"Kroki: {stepCount}";
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            levelText.text = $"Poziom: {sceneIndex}";
        }
    }

    private void NextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels available");
        }
    }

    private void PreviousLevel()
    {
        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if (previousSceneIndex >= 0)
        {
            SceneManager.LoadScene(previousSceneIndex);
        }
        else
        {
            Debug.Log("No previous levels available");
        }
    }

    private void RestartLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
