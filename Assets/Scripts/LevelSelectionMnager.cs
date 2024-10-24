using UnityEngine;

public class LevelSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject levelSelectionUI;  // Reference to the level selection panel
    [SerializeField] private GameObject panelToActivate;   // Panel to activate when a level is selected
    // public LevelLoader levelLoader;      // Reference to the LevelLoader script

    private void Start()
    {
        //  LevelLoader levelLoader = new LevelLoader();
        // Find the LevelLoader script in the scene if it's not assigned in the Inspector
        // if (levelLoader == null)
        // {
        //     levelLoader = FindObjectOfType<LevelLoader>();
        //     if (levelLoader == null)
        //     {
        //         Debug.LogError("LevelLoader script not found in the scene!");
        //     }
        // }
    }

    // public void LoadSelectedLevel(int levelIndex)
    // {
    //     Debug.Log("Level button clicked: " + levelIndex);

    //     // Hide the level selection UI panel
    //     if (levelSelectionUI != null)
    //     {
    //         levelSelectionUI.SetActive(false);
    //         Debug.Log("Level Selection UI set to inactive");
    //     }

    //     // Activate the specified panel
    //     if (panelToActivate != null)
    //     {
    //         panelToActivate.SetActive(true);
    //         Debug.Log("Panel to activate set to active");
    //     }

    //     // Call LoadLevel from LevelLoader
    //     if (levelLoader != null)
    //     {
    //         // StartCoroutine(levelLoader.LoadLevel(levelIndex)); // Load the selected level
    //     }
    //     else
    //     {
    //         Debug.LogError("LevelLoader reference is missing!");
    //     }
    // }
}
