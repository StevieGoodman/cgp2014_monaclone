using System;
using UnityEngine;
using UnityEngine.UI;

public class Main_Menu : MonoBehaviour
{
    public Button continueButton;

    private void Start() => CheckIfContinuePossible();

    public void StartGame()
    {
        // Set the Reputations for all Abilities to 5
        PlayerPrefs.SetInt("HackRep", 5);
        PlayerPrefs.SetInt("DisguiseRep", 5);
        PlayerPrefs.SetInt("KnockoutRep", 5);
        PlayerPrefs.SetInt("LockPickRep", 5);
        
        PlayerPrefs.SetString("CurrentLevel", "Level 1");
        LevelManager.ChangeScene("Bar Scene");
    }

    /// <summary>
    /// Checks if the continue button should be active or not.
    /// </summary>
    private void CheckIfContinuePossible() => continueButton.interactable = PlayerPrefs.GetString("CurrentLevel") != "Level 1";

    public void ContinueGame() =>  LevelManager.ChangeScene("Bar Scene");

    public void QuitGame() => Application.Quit();
}
