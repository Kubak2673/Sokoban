using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    private int stepCount;

    // Method to increment the step count
    public void IncrementStepCount()
    {
        stepCount++;
    }

    // Method to reset the step count
    public void ResetStepCount()
    {
        stepCount = 0;
    }

    // Method to get the current step count
    public int GetStepCount()
    {
        return stepCount;
    }
}
