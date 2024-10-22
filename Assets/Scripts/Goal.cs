using UnityEngine;

public class Goal : MonoBehaviour
{
    private GoalManager goalManager;

    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            goalManager.BoxEnteredGoal();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            goalManager.BoxLeftGoal();
        }
    }
}
