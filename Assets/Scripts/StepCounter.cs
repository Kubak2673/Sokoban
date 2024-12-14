using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StepCounter : MonoBehaviour
{
    public Text stepCounterText;
    public Text levelText;
    public Text didIt;
    private int stepCount = 0;
    private LevelGenerator levelGenerator;
    public GameObject player;
    private Vector3 lastPlayerPosition;

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
            if (Gamepad.current.rightShoulder.wasPressedThisFrame)
            {
                NextLevel();
                return;
            }
            if (Gamepad.current.leftShoulder.wasPressedThisFrame)
            {
                PreviousLevel();
                return;
            }
            if (Gamepad.current.yButton.wasPressedThisFrame)
            {
                RestartLevel();
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            NextLevel();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PreviousLevel();
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

    private void NextLevel()
    {
        levelGenerator.NextLevel();
        UpdateLevelText();
    }

    private void PreviousLevel()
    {
        levelGenerator.PreviousLevel();
        UpdateLevelText();
    }

    private void RestartLevel()
    {
        levelGenerator.RestartLevel();
        UpdateLevelText();
    }
}
