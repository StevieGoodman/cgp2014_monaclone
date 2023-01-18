using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class DisguiseAbility : Ability
{
    private InputAction _actionAsset;
    
    [SerializeField] private float disguiseCountdown;
    public bool IsDisguised => disguiseCountdown > 0;

    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private Sprite disguisedSprite;
    [SerializeField] private Sprite undisguisedSprite;

    public void Awake() => Reputation = PlayerPrefs.GetInt("DisguiseRep");

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
        if (IsDisguised) return;
        if (Charges <= 0) return;
        if (context.started) 
            GetComponent<Interaction>()?.Begin(
                GameManager.Instance.player, 
                context.action.GetParameterValue((HoldInteraction i) => i.duration).Value
            );
        if (context.canceled)
            if (GetComponent<Interaction>().interactionPrompt != null)
                GetComponent<Interaction>().interactionPrompt.OnInteractionInterrupt();
        if (!context.performed) return;
        disguiseCountdown = AbilityLevel switch
        {
            AbilityLevel.Positive => 5,
            AbilityLevel.Neutral => 3,
            AbilityLevel.Negative => 1,
            _ => throw new ArgumentOutOfRangeException()
        };
        Charges--;
        // Gain rep for using this skill.
        AlterReputationValue(1);
        negativeAbility.AlterReputationValue(-1);
        
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
