using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class ChaseBehaviour : MonoBehaviour
{
    private NavMeshAgent _agent;

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
        StartChasing();
    }

    public void StopBehaviour()
    {
        StopAllCoroutines();
    }
}
