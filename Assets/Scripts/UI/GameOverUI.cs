using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;


    public void DisplayGameOver(string gameOverReason)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = gameOverReason;
    }
}
