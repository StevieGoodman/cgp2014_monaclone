using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject _objectivesUI;
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameObject _dialogueUI;
    [SerializeField] private GameObject _skillUI;
    public void OnEscape()
    {
        if (_pauseMenuUI.activeSelf)
            ResumeGame();
        else
            PauseGame();
    }
    public void PauseGame ()
    {
        _pauseMenuUI.SetActive(true);
        //Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        _pauseMenuUI.SetActive(false);
        //Time.timeScale = 1;
    }

    public void ExitButton()
    {
        Application.Quit();
    }

}
