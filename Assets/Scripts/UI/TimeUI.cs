using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    public TextMeshProUGUI timeLabel;

    private void Start()
    {
        GameManager.Instance.timeChanged.AddListener(UpdateTime);
    }

    public void UpdateTime(float time)
    {
        int d = (int)(time * 100.0f);
        int minutes = d / (60 * 100);
        int seconds = (d % (60 * 100)) / 100;
        timeLabel.text = $"{minutes:00}:{seconds:00}";
    }
}
