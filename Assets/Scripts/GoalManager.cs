using UnityEngine;

public class GoalManager : MonoBehaviour
{
    private int totalBoxes;
    private int boxesInGoals;
    private LevelLoader levelLoader;

    void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        totalBoxes = GameObject.FindGameObjectsWithTag("Box").Length;
        boxesInGoals = 0;
    }

    public void BoxEnteredGoal()
    {
        boxesInGoals++;
        CheckAllBoxesInGoals();
    }

    public void BoxLeftGoal()
    {
        boxesInGoals--;
        CheckAllBoxesInGoals();
    }

    private void CheckAllBoxesInGoals()
    {
        Debug.Log($"Checking boxes: {boxesInGoals} out of {totalBoxes}");
        if (boxesInGoals == totalBoxes)
        {
            Debug.Log("Congrats! All boxes are in the goals!");
            levelLoader.OnLevelComplete();
            // Resetting box count for the next level
            boxesInGoals = 0;
        }
    }
}
