using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log(" START GAME ");

        
        PlayerPrefs.SetInt("HackRep", 5);
        PlayerPrefs.SetInt("DisguiseRep", 5);
        PlayerPrefs.SetInt("KnockoutRep", 5);
        PlayerPrefs.SetInt("LockPickRep", 5);
        
        PlayerPrefs.SetString("CurrentLevel", "Level 1");
        LevelManager.ChangeScene("Bar Scene");
    }
    public void QuitGame()
    {
        Debug.Log(" QUIT GAME ");
        Application.Quit();
    }
    
}
