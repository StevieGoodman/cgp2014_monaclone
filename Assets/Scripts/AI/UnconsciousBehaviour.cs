using UnityEngine;
using UnityEngine.AI;

public class UnconsciousBehaviour : MonoBehaviour
{
    
    // Component caches
    private NavMeshAgent _agent;

    // Methods
    private void Awake()
    {
        _agent = GetComponentInChildren<NavMeshAgent>();
    }

    public void LoseConsciousness()
    {
        // Stop the agent from moving to prevent sleepwalking.
        _agent.SetDestination(_agent.transform.position);
        
        // Prevents guards from blocking hallways when unconscious by disabling their colliders.
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.isTrigger = true;
    }
}
