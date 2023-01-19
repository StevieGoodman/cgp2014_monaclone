using System;
using TMPro;
using UnityEngine;

public class AlertUI : MonoBehaviour
{
   public TextMeshProUGUI alertLabel;

   private void Start() =>  AlertSystem.Instance.onAlertLevelChange.AddListener(UpdateAlertLevel);

   private void UpdateAlertLevel()
   {
      var alertLvl = AlertSystem.Instance.AlertLevel.ToString();
      alertLabel.text = alertLvl;
   }
}
