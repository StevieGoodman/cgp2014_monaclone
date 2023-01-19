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
    public AbilityLevel AbilityLevel { get; private set; }

    [Header("Events")]
    public UnityEvent onAbilityUsed;
    public UnityEvent outOfCharges;
    public UnityEvent chargeCountUpdated;
    public UnityEvent reputationValueAltered;
    public UnityEvent<int> repValueChange;
    
    [Header("Configuration")]
    [SerializeField] protected InputActionReference useAction;
    [SerializeField] protected bool holdToPerformAction = false;
    [SerializeField] protected float useRange;
    [SerializeField] protected Ability negativeAbility; // This ability will lose reputation when using this one.

    private void Awake() => UpdateAbilityLevel();

    // This updates the ability level depending on the players reputation towards this ability.
    private void UpdateAbilityLevel() // TODO: Add functions for UI events.
    {
        AbilityLevel = Reputation switch
        {
            > 6 => AbilityLevel.Positive,
            < 4 => AbilityLevel.Negative,
            _ => AbilityLevel.Neutral
        };
    }

    public abstract void UseAbility(InputAction.CallbackContext context);

    public void AlterReputationValue(int value)
    {
        Reputation += value;
        reputationValueAltered?.Invoke();
        repValueChange?.Invoke(value);
        // TODO: Add failure condition when reputation drops below 1.
    }

    /// <summary>
    /// Used to check that the player is looking at a GameObject with a specific component.
    /// </summary>
    /// <param name="raycastHit">Stores the value of the raycast hit object.</param>
    /// <param name="layerMask">The layer mask the raycast will be applied to.</param>
    /// <typeparam name="T">Type of component to search for.</typeparam>
    /// <returns>Whether or not the hit collider's parent <seealso cref="GameObject"/> has <c>T</c> as a component.</returns>
    protected bool HitComponent<T>(out RaycastHit2D raycastHit, LayerMask layerMask)
    {
        raycastHit = Physics2D.Raycast(
            GameManager.Instance.GetPlayerTransform().position,
            GameManager.Instance.GetPlayerTransform().up,
            useRange,
            layerMask);
        if (raycastHit.rigidbody == null) return false;
        return raycastHit.rigidbody.gameObject.GetComponentInParent<T>() != null;
    }
}
