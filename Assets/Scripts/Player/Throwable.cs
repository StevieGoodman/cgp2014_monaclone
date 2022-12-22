using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class Throwable : MonoBehaviour
{
    public float distractionRadius;
    public UnityEvent onCollision;
    private void Start()
    {
        Invoke(nameof(KillMe), 2f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) return;
        // Cause Distraction
        onCollision?.Invoke();
        var AI = col.GetComponentInParent<AIController>();
        if (AI)
            AI.UpdateAIState(AIController.AIState.Unconscious);
        KillMe();
    }

    // Pushes this object using a direction and a throwing force.
    public void ThrowMe(Vector3 direction, float throwForce) 
    {
        GetComponent<Rigidbody2D>().AddRelativeForce(direction.normalized * throwForce, ForceMode2D.Impulse);
    }

    private void KillMe()
    {
        DistractNearbyGuards();
        Destroy(gameObject);
    }

    private void DistractNearbyGuards()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, distractionRadius, transform.up);
        if (hits.Length == 0) return;

        foreach (RaycastHit2D hit in hits)
        {
            // try and get an AI controller.
            var AI = hit.transform.GetComponentInParent<AIController>();
            
            // If we didnt find an AI controller. just go to the next object.
            if (!AI) continue;
            
            // If we did find an AI controller. Set its investigation position to this object.
            // And tell the AI to go investigate it.
            AI.positionToInvestigate = transform.position;
            AI.UpdateAIState(AIController.AIState.Investigating);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distractionRadius);
    }
}
