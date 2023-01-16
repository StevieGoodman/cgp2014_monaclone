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

    // Start is called before the first frame update
    void Start()
    {
       
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTab()
    {
        Debug.Log("! Tab !");
        if (_skillUI.activeSelf == true && _objectivesUI.activeSelf == true)
        {
            _skillUI.SetActive(false);
            _objectivesUI.SetActive(false);
        }
        else
        {
            _skillUI.SetActive(true);
            _objectivesUI.SetActive(true);
        }
    }
    public void OnEscape()
    {
        if (_pauseMenuUI.activeSelf == true)
        {
            _pauseMenuUI.SetActive(false);
            ResumeGame();
        }
        else
        {
            PauseGame();
            _pauseMenuUI.SetActive(true);
            
        }
    }
    private static void PauseGame ()
    {
        Time.timeScale = 0;
    }
    private static void ResumeGame ()
    {
        Time.timeScale = 1;
    }

    public void ResumeButton()
    {
        Debug.Log("Resuming Game!");
        _pauseMenuUI.SetActive(false);
        ResumeGame();
    }

    public void ExitButton()
    {
        Debug.Log("Exiting Game!");
        Application.Quit();
    }

}
