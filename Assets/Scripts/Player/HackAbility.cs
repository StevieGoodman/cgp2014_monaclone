using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class HackAbility : Ability
{
    [SerializeField] private LayerMask layerMask;
    
    public void Awake()
    {
        PlayerPrefs.SetInt("HackReputation", 5);
        Reputation = PlayerPrefs.GetInt("HackReputation");
    }
        
    public override void UseAbility(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!HitComponent<Hackable>(out RaycastHit2D raycastHit, layerMask)) return;
        if (Charges <= 0) return;
        Charges--;
        raycastHit.rigidbody.gameObject.GetComponentInParent<Hackable>().Hack(5);
    }
}
