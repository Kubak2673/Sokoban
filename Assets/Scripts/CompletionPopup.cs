
using UnityEngine;
using UnityEngine.UI;

public class CompletionPopup : MonoBehaviour
{
    public GameObject popupPanel;  // Reference to the popup panel GameObject
    public Text completionText;    // Reference to the Text component for the completion message

    void Start()
    {
        popupPanel.SetActive(false); // Hide the popup at the start
    }

    public void ShowPopup(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        completionText.text = $"{minutes:00}:{seconds:00}";
        popupPanel.SetActive(true); // Show the popup
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}
