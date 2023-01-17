using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;

public class HackAbility : Ability
{
    private InputAction _actionAsset;
    [SerializeField] private LayerMask layerMask;
    
    public void Awake()
    {
        Reputation = PlayerPrefs.GetInt("HackReputation");
    }
    
    private void Start()
    {
        _actionAsset = GetComponent<PlayerInput>().actions["Player/Hack"];
        OnReputationChange();
        reputationValueAltered.AddListener(OnReputationChange);
    }

    private void OnReputationChange()
    {
        float holdDuration = AbilityLevel switch
        {
            AbilityLevel.Positive => 1,
            AbilityLevel.Neutral => 3,
            AbilityLevel.Negative => 5,
            _ => throw new ArgumentOutOfRangeException()
        };
        GetComponent<PlayerInput>().actions["Player/Hack"].ApplyParameterOverride((HoldInteraction interaction) => interaction.duration, holdDuration);
    }
    
    public override void UseAbility(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!HitComponent<Hackable>(out RaycastHit2D raycastHit, layerMask)) return;
        if (Charges <= 0) return;
        Charges--;
        raycastHit.rigidbody.gameObject.GetComponentInParent<Hackable>().Hack(5);
    }
}
