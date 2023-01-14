using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/*
 *  This script is the base for all abilities.
 *  Abilities include a level, and a reputation.
 *
 *  The ability level is what determines your proficiency in an ability.
 *  Positive is good, negative is bad. blah blah blah.
 *
 *  Your reputation level is what determines your ability level.
 *  You gain rep by using the ability. But lose rep another chosen ability by doing so.
 * 
 */
public enum AbilityLevel
{
    Positive = 3,
    Neutral  = 2,
    Negative = 1
}

public abstract class Ability : MonoBehaviour
{

    [Header("Property fields")]
    [SerializeField] private float _charges;
    public float Charges
    {
        get => _charges;
        set => _charges = Math.Clamp(value, 0, (int) AbilityLevel.Positive);
    }
    [SerializeField] private float _reputation;
    public float Reputation
    {
        get => _reputation;
        set {
            _reputation = value;
            UpdateAbilityLevel();
        }
    }
    protected AbilityLevel AbilityLevel { get; private set; }

    [Header("Events")]
    public UnityEvent onAbilityUsed;
    public UnityEvent outOfCharges;
    public UnityEvent chargeCountUpdated;
    
    [Header("Configuration")]
    [SerializeField] protected InputActionReference useAction;
    [SerializeField] protected bool holdToPerformAction = false;
    [SerializeField] protected float useRange;
    [SerializeField] protected Ability negativeAbility; // This ability will lose reputation when using this one.

    public virtual void Update()
    {
        if (useAction.action.triggered) UseAbility();
    }

    // This updates the ability level depending on the players reputation towards this ability.
    private void UpdateAbilityLevel() // TODO: Add functions for UI events.
    {
        Reputation = Mathf.Clamp(Reputation, 1, 10); // Clamp the rep value to make sure its within its correct bounds,
        AbilityLevel = Reputation switch
        {
            > 6 => AbilityLevel.Positive,
            < 4 => AbilityLevel.Negative,
            _ => AbilityLevel.Neutral
        };
    }

    protected abstract void UseAbility();

    public void AlterReputationValue(int value)
    {
        Reputation += value;
        // TODO: Add failure condition when reputation drops below 1.
    }
}
