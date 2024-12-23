using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StepCounter : MonoBehaviour
{
    public Text stepCounterText;
    public Text minStepsText;
    public Text levelText;
    public Text didIt;
    public int stepCount = 0;
    private LevelGenerator levelGenerator;


    private void Start()
    {
        levelGenerator = FindObjectOfType<LevelGenerator>();
        UpdateStepCounter();
        UpdateLevelText();
    }

    private void Update()
    {
        if (Gamepad.current != null)
        {
            if (Gamepad.current.yButton.wasPressedThisFrame)
            {
                RestartLevel();
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    public void IncrementStepCounter()
    {
        stepCount++;
        UpdateStepCounter();
    }
    public void StepCountUpdate()
    {
        // Generate the key for the current level's minimum steps.
        string key = $"minSteps{PlayerPrefs.GetInt("currentLevel")}";
        
        // Check if the key exists in PlayerPrefs.
        if (PlayerPrefs.HasKey(key))
        {
            int storedMinSteps = PlayerPrefs.GetInt(key);
            
            // Update only if the current step count is smaller.
            if (stepCount < storedMinSteps)
            {
                PlayerPrefs.SetInt(key, stepCount);
                minStepsText.text = $"Rekord: {stepCount}";
            }
            // Do nothing if the current step count is not smaller.
        }
        else
        {
            // If no score exists, set the current step count as the minimum.
            PlayerPrefs.SetInt(key, stepCount);
            minStepsText.text = $"Rekord: {stepCount}";
        }
    }
    private void UpdateStepCounter()
    {
        if (stepCounterText != null)
        {
            stepCounterText.text = $"Kroki: {stepCount}";
        }
    }

    private void UpdateLevelText()
    {
        if (levelText != null && levelGenerator != null)
        {
        }
    }

    public void ResetStepCounter()
    {
        stepCount = 0;
        UpdateStepCounter();
    }
    private void RestartLevel()
    {
        levelGenerator.RestartLevel();
        UpdateLevelText();
    }
}
