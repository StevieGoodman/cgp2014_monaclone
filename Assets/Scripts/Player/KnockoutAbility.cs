using UnityEngine;
using UnityEngine.InputSystem;

public class KnockoutAbility : Ability
{
    public float throwRangePositive;
    public float throwRangeNeutral;
    public float throwRangeNegative;

    public GameObject throwablePrefab;

    public void Awake() => Reputation = PlayerPrefs.GetInt("KnockoutReputation");

    public override void UseAbility(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (Charges < 1) return;
        Transform playerPos = GameManager.Instance.GetPlayerTransform();
        Throwable throwable = Instantiate(throwablePrefab, playerPos.position, Quaternion.identity).GetComponent<Throwable>();
        float throwForce = AbilityLevel switch
        {
            AbilityLevel.Positive => throwRangePositive,
            AbilityLevel.Neutral => throwRangeNeutral,
            AbilityLevel.Negative => throwRangeNegative,
            _ => 0f
        };
        throwable.ThrowMe(playerPos.up, throwForce);
        Charges--;
        onAbilityUsed?.Invoke();
        // Gain rep for using this skill.
        AlterReputationValue(1);
        negativeAbility.AlterReputationValue(-1);
    }
}
