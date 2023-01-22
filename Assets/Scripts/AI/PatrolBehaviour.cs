using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolBehaviour : EnemyBehaviour
{
    // The path the agent will follow through. Made of nodes.
    public List<Transform> pathToFollow;
    
    // How long the entity waits once it reaches a node in its path.
    public float waitTime;
    
    [Header("Editor")]
    // The colour of the path the AI will take. Just visualisation stuff
    public Color pathColour;

    // Used to determine if the AI should look in the direction of the node they are at.
    private bool _lookingAround = false;
    
    // Holds the current node the AI is currently at.
    private int _currentNode = 0;
    
    // Holds the previous node that the AI was at.
    private int _lastNode;
    
    
    // Starts the patrolling coroutine.
    public override void StartBehaviour() => StartCoroutine(nameof(GoPatrol));

    private void Update() 
    {
        if (_lookingAround)
            Agent.transform.rotation = Quaternion.RotateTowards(Agent.transform.rotation, pathToFollow[_lastNode].rotation, Time.deltaTime * Agent.angularSpeed);
    }
    
    private IEnumerator GoPatrol()
    {
        UpdatePatrolPath();
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => Agent.remainingDistance <= .1f);
        _lastNode = _currentNode;
        if (_currentNode > pathToFollow.Count - 2)  _currentNode = 0;
        else _currentNode++;
        // Look in the direction of the node the AI is currently at.
        _lookingAround = true;
        yield return new WaitForSeconds(waitTime);
        _lookingAround = false;
        
        StartBehaviour();

        yield return null;
    }
    
    // Sets a new destination for the AI to go to.
    private void UpdatePatrolPath() => Agent.SetDestination(pathToFollow[_currentNode].position);

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        _lookingAround = false;
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
