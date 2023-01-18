using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class ChaseBehaviour : MonoBehaviour
{
    private NavMeshAgent _agent;
    
    // The distance required between the player and this agent to consider the player caught.
    [SerializeField]private float _playerCatchDistance;

    private void Awake()
    {
        _agent = GetComponentInChildren<NavMeshAgent>();
    }

    public void StartChasing()
    {
        StartCoroutine(nameof(Chase));
    }

    public IEnumerator Chase()
    {
        // Update our destination to the player
        _agent.SetDestination(GameManager.Instance.GetPlayerTransform().position);
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        
        if(Vector2.Distance(_agent.transform.position, GameManager.Instance.GetPlayerTransform().position) <= _playerCatchDistance)
            GameManager.Instance.GameOver("Caught!");
        
        StartChasing();
    }

    public void StopBehaviour()
    {
        StopAllCoroutines();
    }
}
