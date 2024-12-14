using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap
    public RuleTile wallRuleTile; // Reference to the RuleTile for walls
    public RuleTile focalPointRuleTile; // Reference to the RuleTile for the focal point (F)
    public GameObject boxPrefab; // Prefab for the boxes
    public GameObject goalPrefab; // Prefab for the goal
    public Camera mainCamera; 
    public static int targetLevelIndex = 0; // Domyślnie ustawiony na 0
    private  Goal goal;
    private Player player; // Reference to the Player script
    private CompletionPopup completionPopup; 
    private StepCounter stepCounter; 
    private string[] currentLevel;
    private int currentLevelIndex = 0;
    private string[][] levelsToLoad;
    private string levelsFilePath = "Assets/Levels/levels.txt";
    public Vector3 focalPoint; // Custom focal point for the camera
    private GameObject playerInstance; // Store player instance to move across levels
    int oldBoxes = 0;
    void Start()
    {
        currentLevelIndex = LevelGenerator.targetLevelIndex; 
        LoadLevelsFromFile(); 
        goal = FindObjectOfType<Goal>(); 
        player = FindObjectOfType<Player>(); LoadLevel(currentLevelIndex);
    }
    void RemoveOldObjectsExceptPlayer()
    {
        foreach (string tag in new[] { "Box", "Goal" })
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag))
            {
                Destroy(obj);
            }
        }
    }
    void LoadLevelsFromFile()
    {
        if (File.Exists(levelsFilePath))
        {
            string[] allLevels = File.ReadAllText(levelsFilePath).Split(new string[] { "---" }, System.StringSplitOptions.RemoveEmptyEntries);
            levelsToLoad = new string[allLevels.Length][];

            for (int i = 0; i < allLevels.Length; i++)
            {
                levelsToLoad[i] = allLevels[i].Split('\n');
            }
        }
    }

 void LoadLevel(int levelIndex)
    {
       
        tilemap.ClearAllTiles();
        oldBoxes = GameObject.FindGameObjectsWithTag("Box").Length;
        RemoveOldObjectsExceptPlayer();
        currentLevel = levelsToLoad[levelIndex];
        GenerateLevel(currentLevel);
        goal = FindObjectOfType<Goal>();
        goal.totalBoxes = GameObject.FindGameObjectsWithTag("Box").Length - oldBoxes;
        SetCameraToLevel(currentLevel);
        MovePlayerToStartPosition(currentLevel);
        stepCounter = FindObjectOfType<StepCounter>();
        stepCounter.ResetStepCounter();
        stepCounter.levelText.text = $"Poziom: {currentLevelIndex+ 1}";
    }


    void GenerateLevel(string[] level)
    {
        int rows = level.Length;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < level[row].Length; col++)
            {
                Vector3Int tilePosition = new Vector3Int(col, -row, 0);
                char tile = level[row][col];
                switch (tile)
                {
                    case '*':
                        Instantiate(boxPrefab, new Vector3(col, -row, 0), Quaternion.identity);
                        break;
                    case 'X':
                        tilemap.SetTile(tilePosition, wallRuleTile);
                        break;
                    case '.':
                        Instantiate(goalPrefab, new Vector3(col, -row, 0), Quaternion.identity);
                        break;
                    case 'f':
                        tilemap.SetTile(tilePosition, focalPointRuleTile);
                        focalPoint = new Vector3(col, -row, 0);
                        break;
                }
            }
        }
    }
  void SetCameraToLevel(string[] level)
    {
        int levelWidth = level[0].Length;
        int levelHeight = level.Length;
        float cameraHeight = levelHeight + 4;
        float cameraWidth = levelWidth + 4;
        mainCamera.orthographicSize = cameraHeight / 2f;
        Vector3 cameraCenter = new Vector3(levelWidth / 2f, -levelHeight / 2f, mainCamera.transform.position.z);
        mainCamera.transform.position = cameraCenter;
        float minX = 0f;
        float maxX = levelWidth;
        float minY = -levelHeight;
        float maxY = 0f;
        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float cameraHalfHeight = mainCamera.orthographicSize;
        cameraCenter.x = Mathf.Clamp(cameraCenter.x, minX + cameraHalfWidth, maxX - cameraHalfWidth);
        cameraCenter.y = Mathf.Clamp(cameraCenter.y, minY + cameraHalfHeight, maxY - cameraHalfHeight);
        mainCamera.transform.position = cameraCenter;
        MoveCameraToFocalPoint(focalPoint);
    }


    void MoveCameraToFocalPoint(Vector3 focalPoint)
    {
        mainCamera.transform.position = new Vector3(focalPoint.x, focalPoint.y, mainCamera.transform.position.z);
    }
    void MovePlayerToStartPosition(string[] level)
    {
        GameObject player = FindObjectOfType<Player>().gameObject;

        if (player != null)
        {
            for (int row = 0; row < level.Length; row++)
            {
                for (int col = 0; col < level[row].Length; col++)
                {
                    if (level[row][col] == '@')
                    {
                        player.transform.position = new Vector3(col, -row, 0);
                        return; // Player moved, exit the function
                    }
                }
            }
        }
    }
    public void NextLevel()
    {
        if (currentLevelIndex < levelsToLoad.Length - 1)
        {
            completionPopup = FindObjectOfType<CompletionPopup>();
            CompletionPopup.isLevelCompleted = false;
            stepCounter = FindObjectOfType<StepCounter>();
            currentLevelIndex++;
            stepCounter.levelText.text = $"Poziom: {currentLevelIndex+1}";
            player.ResetUndoHistory();            
            goal = FindObjectOfType<Goal>();
            LoadLevel(currentLevelIndex);
            stepCounter.didIt.text = "";
            stepCounter.didIt.color = Color.white;
            stepCounter.levelText.color = Color.white;
        }
    }
    public void PreviousLevel()
    {
        if (currentLevelIndex > 0)
        {
            completionPopup = FindObjectOfType<CompletionPopup>();
            CompletionPopup.isLevelCompleted = false;
            stepCounter = FindObjectOfType<StepCounter>();
            currentLevelIndex--;
            stepCounter.levelText.text = $"Poziom: {currentLevelIndex+ 1}";
            player.ResetUndoHistory();            
            goal = FindObjectOfType<Goal>();
            LoadLevel(currentLevelIndex);
            stepCounter.didIt.text = "";
            stepCounter.didIt.color = Color.white;
            stepCounter.levelText.color = Color.white;
        }
    }

    public void RestartLevel()
    {
        player.ResetUndoHistory();
        goal = FindObjectOfType<Goal>();
        LoadLevel(currentLevelIndex);
        stepCounter.didIt.text = "";
        stepCounter.didIt.color = Color.white;
        stepCounter.levelText.color = Color.white;
    }

    public void UpdateFocalPoint(Vector3 newFocalPoint)
    {
        focalPoint = newFocalPoint;
        SetCameraToLevel(currentLevel);
    }
    public int GetCurrentLevelIndex() 
    { 
        return currentLevelIndex; 
    }
}