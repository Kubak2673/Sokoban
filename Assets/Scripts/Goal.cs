using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private static int boxesInGoal = 0;
    private static int totalBoxes;
    private LevelGenerator levelGenerator; 
    void Start()
    {
        totalBoxes = GameObject.FindGameObjectsWithTag("Box").Length;
        boxesInGoal = 0;
        levelGenerator = FindObjectOfType<LevelGenerator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            // Change the box's sprite to the "on goal" sprite
            SpriteRenderer boxSpriteRenderer = other.GetComponent<SpriteRenderer>();
            BoxMovement boxMovement = other.GetComponent<BoxMovement>();
            if (boxSpriteRenderer != null && boxMovement != null)
            {
                boxSpriteRenderer.sprite = boxMovement.onGoalSprite;
            }

            boxesInGoal++;
            Debug.Log("Box wkroczył na goal");
            CheckAllBoxesInGoal();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            // Revert the box's sprite to the default sprite
            SpriteRenderer boxSpriteRenderer = other.GetComponent<SpriteRenderer>();
            BoxMovement boxMovement = other.GetComponent<BoxMovement>();
            if (boxSpriteRenderer != null && boxMovement != null)
            {
                boxSpriteRenderer.sprite = boxMovement.defaultSprite;
            }

            boxesInGoal--;
            Debug.Log("Box opuścił goal");
        }
    }

    private void CheckAllBoxesInGoal()
    {
        if (boxesInGoal == totalBoxes)
        {
            levelGenerator.NextLevel();
        }
    }
}
