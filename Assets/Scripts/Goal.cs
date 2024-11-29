using UnityEngine;
using UnityEngine.SceneManagement;
public class Goal : MonoBehaviour
{
    private static int boxesInGoal = 0;
    private static int totalBoxes;
    void Start()
    {
        totalBoxes = GameObject.FindGameObjectsWithTag("Box").Length;
        boxesInGoal = 0;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            boxesInGoal++;
            Debug.Log("Box wkroczył na goal");
            CheckAllBoxesInGoal();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            boxesInGoal--;
            Debug.Log("Box opuścił goal");
        }
    }
    private void CheckAllBoxesInGoal()
    {
        if (boxesInGoal == totalBoxes)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(currentSceneIndex + 1);
            }
            else
            {
                Debug.Log("Nie ma następnego poziomu");
            }
        }
    }
}
