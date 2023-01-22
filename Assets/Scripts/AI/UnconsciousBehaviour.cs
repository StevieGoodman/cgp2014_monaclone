using UnityEngine;
using UnityEngine.AI;

public class UnconsciousBehaviour : EnemyBehaviour
{
    
    public override void StartBehaviour()
    {
        // Stop the agent from moving to prevent sleepwalking.
        Agent.SetDestination(Agent.transform.position);
        
        ToggleColliders(true);
    }

    public override void StopBehaviour() => ToggleColliders(false);
    
    private void ToggleColliders(bool active)
    {
        // Enables guards' colliders again to restore collision behaviour.
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.isTrigger = active;
    }
}
