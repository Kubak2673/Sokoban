using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CompletionPopup : MonoBehaviour
{
    public GameObject popupPanel;           // The popup panel UI        // The text field in the popup
    private StepCounter stepCounter; 
    private LevelGenerator levelGenerator;
    public static bool isLevelCompleted = false; 
    private void Start()
    {
        popupPanel.SetActive(false);
    }
    public void ShowCompletionPopup()
    {
        isLevelCompleted = true;
        stepCounter = FindObjectOfType<StepCounter>();
        stepCounter.didIt.text = "ukończono";
        stepCounter.didIt.color = Color.green;
        stepCounter.levelText.color = Color.green;
        levelGenerator = FindObjectOfType<LevelGenerator>();
        StartCoroutine(levelGenerator.NextLevel());
    }
} 