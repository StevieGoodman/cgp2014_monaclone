using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

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
public class Ability : MonoBehaviour
{
    public enum AbilityLevel
    {
        Positive,
        Neutral,
        Negative
    }

    public InputActionReference useAction;
    public AbilityLevel abilityLevel;
    [Range(1, 10)]public float reputation;
    public float useRange;
    [Space]
    public Ability negativeAbility; // This ability will lose reputation when using this one.

    [Space]
    // Abilities have use limits. These are the current amount of uses and the uses for each ability level.
    public int charges;
    public int positiveCharges;
    public int neutralCharges;
    public int negativeCharges;

    [Header("Ability Events")]
    public UnityEvent onAbilityUsed;
    public UnityEvent outOfCharges;
    public UnityEvent chargeCountUpdated;

    // This is called when the script instance is loading.
    public virtual void Awake()
    {

    }

    public void Start()
    {
        UpdateChargeLimits();
    }

    public virtual void Update()
    {
        if (useAction.action.triggered) UseAbility();
    }

    // This updates the ability level depending on the players reputation towards this ability.
    public virtual void UpdateAbilityLevel() // TODO: Add functions for UI events.
    {
        reputation = Mathf.Clamp(reputation, 1, 10); // Clamp the rep value to make sure its within its correct bounds,
        switch (reputation)
        {
            case > 6:
                abilityLevel = AbilityLevel.Positive;
                break;
            case < 4:
                abilityLevel = AbilityLevel.Negative;
                break;
            default:
                abilityLevel = AbilityLevel.Neutral;
                break;
        }
        UpdateChargeLimits(); // Set new charge limits after updating the level of this ability.
    }

    public virtual void UpdateChargeLimits()
    {
        // Apply the correct amount of charges to the ability depending on level.
        switch (abilityLevel)
        {
            case AbilityLevel.Positive:
                charges = positiveCharges;
                break;
            case AbilityLevel.Neutral:
                charges = neutralCharges;
                break;
            case AbilityLevel.Negative:
                charges = negativeCharges;
                break;
        }
    }
    public virtual void UseAbility()
    {
        // Ability code goes here.
    }
}
