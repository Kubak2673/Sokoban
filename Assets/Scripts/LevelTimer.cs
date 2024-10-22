using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    public Text timerText;  // Reference to the UI Text component
    private float levelTime;
    private bool isTimerRunning = false;

    void Update()
    {
        if (isTimerRunning)
        {
            levelTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(levelTime / 60F);
        int seconds = Mathf.FloorToInt(levelTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        isTimerRunning = false;  // Stop the timer
    }

    public void StartTimer()
    {
        isTimerRunning = true;  // Start the timer
    }

    public void ResetTimer()
    {
        levelTime = 0f;  // Reset the timer value
        UpdateTimerText();  // Update the UI
    }

    public float GetLevelTime()
    {
        return levelTime;  // Return the current level time
    }
}
