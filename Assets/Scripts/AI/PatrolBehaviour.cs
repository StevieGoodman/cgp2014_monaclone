using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PatrolBehaviour : MonoBehaviour
{
    // The path the agent will follow through. Made of nodes.
    public List<Transform> pathToFollow;
    
    // How long the entity waits once it reaches a node in its path.
    public float waitTime;
    
    
    [Header("Editor")]
    // The colour of the path the AI will take. Just visualisation stuff
    public Color pathColour;

    // The controller that switches between each AI state.
    private AIController _aiController;
    
    // The agent component moves our entity through their patrol path.
    private NavMeshAgent _agent;
    
    // Used to determine if the AI should look in the direction of the node they are at.
    private bool _lookingAround = false;
    
    // Holds the current node the AI is currently at.
    private int _currentNode = 0;
    
    // Holds the previous node that the AI was at.
    private int _lastNode;
    
    private void Awake()
    {
        // Get required Components.
        _aiController = GetComponent<AIController>();
        _agent = GetComponentInChildren<NavMeshAgent>();

    }

    // Starts the patrolling coroutine.
    public void StartPatrolling()
    {
        StartCoroutine(nameof(GoPatrol));
    }

    private void Update() 
    {
        if (_lookingAround)
            _agent.transform.rotation = Quaternion.RotateTowards(_agent.transform.rotation, pathToFollow[_lastNode].rotation, Time.deltaTime * _agent.angularSpeed);
    }
    // Patrolling coroutine that has the AI navigate from node to node.
    private IEnumerator GoPatrol()
    {
        UpdatePatrolPath();
        yield return new WaitForSeconds(0.1f); // For some reason. This is required. If its not here the AI instantly thinks the remaining distance is done.
        yield return new WaitUntil(() => _agent.remainingDistance <= .1f);
        _lastNode = _currentNode; // Define a last node so we can define our next node and still match the rotation of the one we currently are at.
        if (_currentNode > pathToFollow.Count - 2)  _currentNode = 0; // If the current node would be bigger than the node array. we want to go back to 0.
        else _currentNode++;
        // Look in the direction of the node the AI is currently at.
        _lookingAround = true;
        yield return new WaitForSeconds(waitTime);
        _lookingAround = false;
        
        StartPatrolling();
    }
    // Sets a new destination for the AI to go to.
    private void UpdatePatrolPath()
    {
        _agent.SetDestination(pathToFollow[_currentNode].position);
    }
    private void OnDrawGizmos()
    {
        // Used to draw out a rough path the AI will take in the editor.
        if (pathToFollow.Count <= 1) return;
        Gizmos.color = pathColour;
        
        for (var i = 0; i < pathToFollow.Count; i++)
            Gizmos.DrawLine(pathToFollow[i].transform.position, i + 1 < pathToFollow.Count ? pathToFollow[i + 1].transform.position : pathToFollow[0].transform.position);
    }
}
