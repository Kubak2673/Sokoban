using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Required for the new Input System

public class StepCounter : MonoBehaviour
{
    public Text stepCounterText;
    public Text levelText;
    private int stepCount = 0;
 private LevelGenerator levelGenerator; 
    public GameObject player; // Reference to the player object
    private Vector3 lastPlayerPosition;

    private void Start()
    {
        // Set initial values
        UpdateStepCounter();
        UpdateLevelText();
        levelGenerator = FindObjectOfType<LevelGenerator>();
        // Find the player object if not set
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Store the player's initial position
        if (player != null)
        {
            lastPlayerPosition = player.transform.position;
        }
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

        // Count steps based on player movement
        if (player != null)
        {
            // If the player has moved, increment the step counter
            if (player.transform.position != lastPlayerPosition)
            {
                IncrementStepCounter();
                lastPlayerPosition = player.transform.position;
            }
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
        levelGenerator.NextLevel();
    }

    private void PreviousLevel()
    {
        levelGenerator.PreviousLevel();
    }

    private void RestartLevel()
    {
        levelGenerator.RestartLevel();
    }
}
