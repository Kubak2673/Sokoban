using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private static int boxesInGoal = 0; // Shared counter across all goals
    private static int totalBoxes;      // Total boxes in the current scene

    void Start()
    {
        // Recalculate total boxes for the new scene
        totalBoxes = GameObject.FindGameObjectsWithTag("Box").Length;

        // Reset the counter for boxes in goal
        boxesInGoal = 0;

        // Optionally log the start state
        Debug.Log($"Level {SceneManager.GetActiveScene().buildIndex} started. Total Boxes: {totalBoxes}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            boxesInGoal++;
            Debug.Log($"Box entered goal. Boxes in Goal: {boxesInGoal}/{totalBoxes}");
            CheckAllBoxesInGoal();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            boxesInGoal--;
            Debug.Log($"Box exited goal. Boxes in Goal: {boxesInGoal}/{totalBoxes}");
        }
    }

    private void CheckAllBoxesInGoal()
    {
        if (boxesInGoal == totalBoxes)
        {
            Debug.Log("All boxes in goal! Level complete.");

            // Call CompleteLevel to update high score and save it
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        // Ensure the StepCounter script exists in the scene
        StepCounter stepCounter = FindObjectOfType<StepCounter>();
        if (stepCounter != null)
        {
            stepCounter.CheckAndSaveHighScore(); // Check and save the high score
        }

        // Optionally load the next level or complete the level
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if there is a next level to load
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex); // Load next level
        }
        else
        {
            Debug.Log("No more levels. Game Over or Main Menu.");
            // Optionally add logic to go to the main menu or show a game complete screen.
        }
    }
}
