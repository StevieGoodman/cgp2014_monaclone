using System;
using UnityEngine;
using UnityEngine.Serialization;


public class UIManager : MonoBehaviour
{
    [FormerlySerializedAs("reputationBarUI")] public AbilityUI abilityUI;
    public ObjectiveUI objectiveUI;
    
    [SerializeField] private GameObject _objectivesUI;
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameObject _dialogueUI;
    [SerializeField] private GameObject _skillUI;

    private void Awake()
    {
        if (!abilityUI) abilityUI = GetComponent<AbilityUI>();
        if (!objectiveUI) objectiveUI = GetComponent<ObjectiveUI>();
    }

    public void OnEscape()
    {
        if (_pauseMenuUI.activeSelf)
            ResumeGame();
        else
            PauseGame();
    }
    private void PauseGame()
    {
        _pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
    }
    public void ExitButton() => Application.Quit();
}
