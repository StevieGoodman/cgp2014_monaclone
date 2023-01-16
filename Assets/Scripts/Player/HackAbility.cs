using UnityEngine;
using UnityEngine.InputSystem;

public class HackAbility : Ability
{
    [SerializeField] private LayerMask _layerMask;
    
    public override void UseAbility(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        RaycastHit2D raycastHit = Physics2D.Raycast(
            GameManager.Instance.GetPlayerTransform().position,
            GameManager.Instance.GetPlayerTransform().up,
            useRange,
            _layerMask);
        if (raycastHit.rigidbody == null) return;
        if (raycastHit.rigidbody.gameObject.GetComponentInParent<Hackable>() == null) return;
        if (Charges <= 0) return;
        Charges--;
        raycastHit.rigidbody.gameObject.GetComponentInParent<Hackable>().Hack(5);
        Debug.Log("Hacked!");
    }
}
