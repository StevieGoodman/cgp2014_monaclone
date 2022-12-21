using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PatrolBehaviour : MonoBehaviour
{
    public Transform[] pathToFollow; // TODO: Make this automatically collect the path. 
    public float waitTime;
    
    [Header("Editor")]
    public Color pathColour; // The colour of the path the AI will take. Just visualisation stuff
    
    private NavMeshAgent _agent;
    private AIController _aiController;
    private bool _lookingAround = false;
    private int _currentNode = 0;
    private int _lastNode;

    private void Awake()
    {
        _aiController = GetComponent<AIController>();
        _agent = GetComponentInChildren<NavMeshAgent>();
    }

    public void StartPatrolling()
    {
        StartCoroutine(nameof(GoPatrol));
    }

    private void Update() 
    {
        if (_lookingAround)
            _agent.transform.rotation = Quaternion.RotateTowards(_agent.transform.rotation, pathToFollow[_lastNode].rotation, Time.deltaTime * _aiController.angularSpeed);
    }
 

    private IEnumerator GoPatrol()
    {
        UpdatePatrolPath();
        yield return new WaitForSeconds(0.1f); // For some reason. This is required. If its not here the AI instantly thinks the remaining distance is done.
        yield return new WaitUntil(() => _agent.remainingDistance <= .1f);
        _lastNode = _currentNode; // Define a last node so we can define our next node and still match the rotation of the one we currently are at.
        if (_currentNode > pathToFollow.Length - 2)  _currentNode = 0; // If the current node would be bigger than the node array. we want to go back to 0.
        else _currentNode++;
        // Look in the direction of the node the AI is currently at.
        _lookingAround = true;
        yield return new WaitForSeconds(waitTime);
        _lookingAround = false;
        if (_aiController.aiState == AIController.AIState.Patrolling)
            StartPatrolling();
    }

    private void UpdatePatrolPath()
    {
        _agent.SetDestination(pathToFollow[_currentNode].position);
    }
    private void OnDrawGizmos()
    { // Used to draw out a rough path the AI will take in the editor.
        if (pathToFollow.Length <= 1) return;
        Gizmos.color = pathColour;
        
        for (var i = 0; i < pathToFollow.Length; i++)
            Gizmos.DrawLine(pathToFollow[i].transform.position, i + 1 < pathToFollow.Length ? pathToFollow[i + 1].transform.position : pathToFollow[0].transform.position);
    }
}
