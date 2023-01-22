using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    protected NavMeshAgent Agent;

    public virtual void Awake() => GetComponents();

    protected virtual void GetComponents() => Agent = GetComponentInChildren<NavMeshAgent>();
    
    /// <summary>
    /// Starts the logic in this behaviour.
    /// </summary>
    public virtual void StartBehaviour() {}
    
    /// <summary>
    /// Starts the logic in this behaviour. Overloaded to take position as input.
    /// </summary>
    /// <param name="position">Position in the world</param>
    public virtual void StartBehaviour(Vector3 position = default) {}

    /// <summary>
    /// Stops The logic in this behaviour.
    /// </summary>
    public virtual void StopBehaviour() { StopAllCoroutines();}

}
