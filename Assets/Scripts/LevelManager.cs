using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int whatLevel;
     public void OnButtonClick()
    {
        SceneManager.LoadScene(whatLevel); 
    }
    
}