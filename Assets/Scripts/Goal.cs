using UnityEngine;
using System.Collections;
public class Goal : MonoBehaviour
{
    public static int boxesInGoal = 0;
    public static int totalBoxes = 0;
    private CompletionPopup completionPopup;

    private LevelGenerator levelGenerator;
    void Start()
    {
        levelGenerator = FindObjectOfType<LevelGenerator>();
        Debug.Log($"Total Boxes: {totalBoxes}");
        Collider2D goalCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            Debug.Log("Box entered the goal");
            SpriteRenderer boxSpriteRenderer = other.GetComponent<SpriteRenderer>();
            BoxMovement boxMovement = other.GetComponent<BoxMovement>();
            boxMovement.StartOnGoal();
            boxesInGoal++;
            CheckAllBoxesInGoal();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            Debug.Log("Box exited the goal");
            SpriteRenderer boxSpriteRenderer = other.GetComponent<SpriteRenderer>();
            BoxMovement boxMovement = other.GetComponent<BoxMovement>();
            boxMovement.StartLeftGoal();
            boxesInGoal--;
        }
    }
    public void CheckAllBoxesInGoal()
    {
        Debug.Log($"Checking if all boxes are in the goal: {boxesInGoal}/{totalBoxes}");
        if (boxesInGoal == totalBoxes)
        {
            Debug.Log("All boxes are in the goal!");
            completionPopup = FindObjectOfType<CompletionPopup>();
            completionPopup.ShowCompletionPopup();
            if (LevelManager.currLevel == LevelManager.UnlockedLevels)
            {
                LevelManager.UnlockedLevels++;
                PlayerPrefs.SetInt("UnlockedLevels", LevelManager.UnlockedLevels);
            }
        }
        else 
        {
            return;
        }
    }
}