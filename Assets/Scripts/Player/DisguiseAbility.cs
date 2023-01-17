using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;

public class DisguiseAbility : Ability
{
    private InputAction _actionAsset;
    
    [SerializeField] private float disguiseCountdown;
    public bool IsDisguised => disguiseCountdown > 0;

    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private Sprite disguisedSprite;
    [SerializeField] private Sprite undisguisedSprite;

    private void Start()
    {
        _actionAsset = GetComponent<PlayerInput>().actions["Player/Disguise"];
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
        GetComponent<PlayerInput>().actions["Player/Disguise"].ApplyParameterOverride((HoldInteraction interaction) => interaction.duration, holdDuration);
    }
    
    public override void UseAbility(InputAction.CallbackContext context)
    {
        Debug.Log(context.action.GetParameterValue((HoldInteraction i) => i.duration));
        if (!context.performed) return;
        if (IsDisguised) return;
        if (Charges <= 0) return;
        disguiseCountdown = AbilityLevel switch
        {
            AbilityLevel.Positive => 5,
            AbilityLevel.Neutral => 3,
            AbilityLevel.Negative => 1,
            _ => throw new ArgumentOutOfRangeException()
        };
        Charges--;
        StartCoroutine(Disguise());
    }

    private IEnumerator Disguise()
    {
        playerSpriteRenderer.sprite = disguisedSprite;
        while (disguiseCountdown > 0)
        {
            yield return null;
            disguiseCountdown -= Time.deltaTime;
        }
        playerSpriteRenderer.sprite = undisguisedSprite;
    }
}
