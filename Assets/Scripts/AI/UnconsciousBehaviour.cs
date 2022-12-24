using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnconsciousBehaviour : MonoBehaviour
{
    private NavMeshAgent _agent;
    
    
    private void Awake()
    {
        _agent = GetComponentInChildren<NavMeshAgent>();
    }

    public void LoseConciousness()
    {
        // Do some knock out animation.
        
        // Stop the AI where he is and disable his navMeshAgent
        _agent.SetDestination(_agent.transform.position);
        
        // Disable all colliders for this Guard.
        foreach (var col in GetComponentsInChildren<Collider2D>())
        {
            col.isTrigger = true;
        }
    }
}
