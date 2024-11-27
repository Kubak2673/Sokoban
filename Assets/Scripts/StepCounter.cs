using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StepCounter : MonoBehaviour
{
    public Text stepCounterText;
    public Text highScoreText;
    private string levelName;
    private int stepCount = 0;
    private int highScore = int.MaxValue;
    private string filePath;

    private void Start()
    {
        levelName = SceneManager.GetActiveScene().name;
        filePath = Path.Combine(Application.persistentDataPath, $"{levelName}_highscore.json");

        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
        Debug.Log("Current Level: " + levelName);

        LoadHighScore();
        UpdateStepCounter();
        UpdateHighScoreDisplay();
    }

    public void IncrementStepCounter()
    {
        stepCount++;
        UpdateStepCounter();
    }

    public void UndoMove()
    {
        Debug.Log("Undo move - Step count unchanged.");
    }

    public void CheckAndSaveHighScore()
    {
        if (stepCount < highScore)
        {
            highScore = stepCount;
            SaveHighScore();
            UpdateHighScoreDisplay();
        }
    }

    private void UpdateStepCounter()
    {
        if (stepCounterText != null)
            stepCounterText.text = $"Steps: {stepCount}";
    }

    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
            highScoreText.text = $"Best: {highScore} steps";
    }

    private void LoadHighScore()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);
            highScore = data.highScore;

            Debug.Log("High Score Loaded: " + highScore);
        }
        else
        {
            Debug.Log("No high score file found. Starting with default high score.");
            highScore = int.MaxValue;
        }
    }

    private void SaveHighScore()
    {
        HighScoreData data = new HighScoreData { highScore = highScore };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"High score saved for {levelName}: {highScore} steps");
    }

private void OnApplicationQuit()
{
    // No need to save high score on application quit anymore
    // This method is now redundant
}


    [System.Serializable]
    private class HighScoreData
    {
        public int highScore;
    }
}