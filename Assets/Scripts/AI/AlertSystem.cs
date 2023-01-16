using UnityEngine;
public class AlertSystem : MonoBehaviour
{
    // Singleton Alert System Instance.
    public static AlertSystem Instance;
    
    public enum AlertnessLevel
    {
        low,
        medium,
        high
    }
    
    // The alert level of this scene.
    [SerializeField] private AlertnessLevel _alertnessLevel = AlertnessLevel.low; // Always starts at low.

    public AlertnessLevel AlertLevel
    {
        get => _alertnessLevel;
        set{
            _alertnessLevel = value;
            UpdateAIStats();
        }
    }

    // Tokens are like strikes. Once you reach a certain amount, the alert level jumps up.
    [SerializeField]private int _tokens;

    /// <summary>
    /// The amount of times the Player has been detected, when the player accumulates
    /// enough tokens, the alert level increases.
    /// </summary>
    public int Tokens
    {
        get => _tokens;
        set
        {
            _tokens = value;
            if (value == mediumTokenRequirement) AlertLevel = AlertnessLevel.medium;
            if (value == highTokenRequirement) AlertLevel = AlertnessLevel.high;
            
        }
    }
    
    // The amount of tokens required to alter the alert level.
    public int mediumTokenRequirement;
    public int highTokenRequirement;
    
    private void Awake() => Instance = this;

    // Updates the alertness of all AI.
    private void UpdateAIStats()
    {
        Debug.Log("AI Alertness Changed to: " + _alertnessLevel);
        foreach (var ai in GameManager.Instance.aiControllers)
            ai.UpdateAIAlertness(_alertnessLevel);
    }
}
