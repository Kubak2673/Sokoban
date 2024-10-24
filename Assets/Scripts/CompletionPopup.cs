using UnityEngine;
using UnityEngine.UI;

public class CompletionPopup : MonoBehaviour
{
    public GameObject popupPanel;  // Reference to the popup panel GameObject
    // public Text completionText;    // Reference to the Text component for the completion message
   
    void Start()
    {
        popupPanel.SetActive(false); // Hide the popup at the start
    }

    public void ShowPopup(int steps)
    {
        // Show the number of steps in the completion message
        // completionText.text = $"Level Complete!\nSteps: {steps}";
        // stepsText.text = $"Steps: {steps}"; // Update steps text
        popupPanel.SetActive(true); // Show the popup
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }

    // public void UpdateStepCountText(int steps)
    // {
    //    Debug.Log("Current step count: " + steps);
    //     stepsText.text = $" s:{steps}"; // Update steps text
    // }
}
