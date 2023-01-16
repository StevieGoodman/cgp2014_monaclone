using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisguiseAbility : Ability
{

    [SerializeField] private float disguiseCountdown;
    public bool IsDisguised => disguiseCountdown > 0;

    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private Sprite disguisedSprite;
    [SerializeField] private Sprite undisguisedSprite;

    public override void UseAbility(InputAction.CallbackContext context)
    {
        if (!context.started) return;
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
