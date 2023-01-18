using System;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    public ReputationBarUI reputationBarUI;
    public ObjectiveUI objectiveUI;
    
    [SerializeField] private GameObject _objectivesUI;
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameObject _dialogueUI;
    [SerializeField] private GameObject _skillUI;

    private void Awake()
    {
        if (!reputationBarUI) reputationBarUI = GetComponent<ReputationBarUI>();
        if (!objectiveUI) objectiveUI = GetComponent<ObjectiveUI>();
    }

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
