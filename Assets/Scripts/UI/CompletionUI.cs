using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CompletionUI : MonoBehaviour
{ 
    public GameObject completionPanel;
    
    public void DisplayCompletion()
    {
        completionPanel.SetActive(true);
    }
}
