using UnityEngine;
using UnityEngine.Events;

public class Throwable : MonoBehaviour
{
    // How far can the throwable distract AI?
    public float distractionRadius;
    
    // Event for when the throwable collides with something.
    public UnityEvent onCollision;
    
    private void Start()
    {
        Invoke(nameof(KillMe), 2f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // If we collide with a player, we want to ignore the collision.
        if (col.CompareTag("Player")) return;

        // Call the onCollision event.
        onCollision?.Invoke();
        
        // If we find an AI Controller on the thing we collided with, knock them out.
        var AI = col.GetComponentInParent<AIController>();
        if (AI) AI.UpdateAIState(AIController.AIState.Unconscious);

        // Destroy this object.
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
        // Do a physics cast around the throwable.
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

    private void OnDrawGizmosSelected()
    {
        // Shows the distraction radius of the object.
        Gizmos.DrawWireSphere(transform.position, distractionRadius);
    }
}
