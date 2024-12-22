using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StepCounter : MonoBehaviour
{
    public Text stepCounterText;
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
