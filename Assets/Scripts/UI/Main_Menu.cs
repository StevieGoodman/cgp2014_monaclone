using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log(" START GAME ");
        SceneManager.LoadScene("Bar Scene");
    }
    public void QuitGame()
    {
        Debug.Log(" QUIT GAME ");
        Application.Quit();
    }
    
}
