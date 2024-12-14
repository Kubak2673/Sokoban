using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int levelIndex; // The level index for this button

    public void OnButtonClick()
    {
        // Set the target level index
        LevelGenerator.targetLevelIndex = levelIndex;

        // Load scene 1
        SceneManager.LoadScene(1);
    }
}
