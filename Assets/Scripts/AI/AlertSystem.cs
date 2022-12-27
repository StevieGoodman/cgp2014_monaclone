using System;
using UnityEngine;


/* ALERT SYSTEM
 *
 * This system alters how sensitive the AI is to the players actions. The more the player disrupts them,
 * the more alert and brutal the guards will be.
 *
 * How does this system work?
 * There are three alert levels, low, medium and high. Each level alters how guards and other units respond
 * to the players presence. Guards on a high alert level will notice the player alot quicker and will try harder
 * to track down the player.
 *
 * Every time the player does the following, a token is added to this system:
 *
 * - Guard sees and chases the player.
 *
 * - Security camera sees the player.
 *
 * - Guard gets up after being unconscious.
 *
 *  If the player earns too many tokens, the alert level raises. Increasing the alertness of all AI.
 */
public class AlertSystem : MonoBehaviour
{
    // The alert level of this scene.
    public AlertLevel alertLevel = AlertLevel.low; // Always starts at low.
    public enum AlertLevel
    {
        low,
        medium,
        high
    }

    // Tokens are like strikes. Once you reach a certain amount, the alert level jumps up.
    public int tokens;

    // How many tokens is required to reach medium alert level.
    public int mediumTokenRequirement;
    
    // How many tokens is required to reach medium alert level.
    public int highTokenRequirement;

    // Singleton Alert System Instance.
    public static AlertSystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void IncreaseAlertTokens(int amount = 1)
    {
        // Add tokens.
        tokens += amount;

        if (tokens == mediumTokenRequirement)
        {
            alertLevel = AlertLevel.medium;
            UpdateAIStats();
        }

        if (tokens == highTokenRequirement)
        {
            alertLevel = AlertLevel.high;
            UpdateAIStats();
        }
    }
    // Updates the alertness of all AI.
    private void UpdateAIStats()
    {
        Debug.Log("AI Alertness Changed to: " + alertLevel);
        foreach (var ai in GameManager.Instance.aiControllers)
            ai.UpdateAIAlertness(alertLevel);
    }
}
