using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    public LevelObject[] levelObjects;
    public static int UnlockedLevels;
    public static int currLevel; // The level index for this button
    private void Start() {
        UnlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 0);
        for ( int i = 0; i < levelObjects.Length; i++ )
        {
            if ( UnlockedLevels >= i )
            {
                levelObjects[i].levelButton.interactable = true; 
            }
        }
    }
    public void OnClickLevel(int levelNum)
    {
        currLevel = levelNum;
        LevelGenerator.targetLevelIndex = levelNum;
        SceneManager.LoadScene(1);
    }
    public void OnClickClearAndLevel()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }
}
