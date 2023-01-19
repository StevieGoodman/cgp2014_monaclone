using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class InvestigateBehaviour : MonoBehaviour
{
    // How long should the AI investigate a location for before going back to other tasks?
    public float lookAroundTime;
    
    // Investigation start and end events
    public UnityEvent onInvestigationStart;
    public UnityEvent onInvestigationEnd;
    
    private AIController _aiController;
    private NavMeshAgent _agent;
    private bool _lookAround;
    private Quaternion _randomRot;

    private void Awake()
    {
        // Get the required components to get this Script running.
        _aiController = GetComponent<AIController>();
        _agent = GetComponentInChildren<NavMeshAgent>();
        
        if(!_aiController)
            Debug.LogError(gameObject + " cannot find a AIController! Make sure there is one on it.");
        
        if(!_agent)
            Debug.LogError(gameObject + " cannot find a NavMeshAgent! Make sure there is one on it.");
    }
    
    private void Update()
    {
        if(_lookAround) 
            _agent.transform.rotation = Quaternion.RotateTowards(_agent.transform.rotation, _randomRot, Time.deltaTime * _agent.angularSpeed);
    }

    public void GoInvestigatePosition(Vector3 posToInvestigate) // This just allows you to call the coroutine from other scripts
    {
        StartCoroutine(nameof(InvestigatePosition), posToInvestigate);
    }
    private IEnumerator InvestigatePosition(Vector3 posToInvestigate)
    {
        UpdateAgentPosition(_agent.transform.position);
        
        // Call investigation start event.
        onInvestigationStart?.Invoke();
        
        yield return new WaitForSeconds(.2f);
        UpdateAgentPosition(posToInvestigate);
        
        // Fixes an issue where the wait until condition was immediately satisfied.
        yield return new WaitForSeconds(0.1f);
        
        // Forcefully ends the investigation if the condition isn't met in 10 seconds.
        StartCoroutine(ForcefullyFailInvestigation());
        
        yield return new WaitUntil(() => _agent.remainingDistance < 0.1f);
        
        // Look around in random directions for a bit.
        _lookAround = true;
        _randomRot = (Random.rotation);
        yield return new WaitForSeconds(lookAroundTime / 2);
        _randomRot = (Random.rotation);
        yield return new WaitForSeconds(lookAroundTime / 2);
        _lookAround = false;
        
        if(_aiController)
            _aiController.UpdateAIState(AIController.AIState.Patrolling);
        
        onInvestigationEnd?.Invoke();
        StopAllCoroutines(); 
    }
    private void UpdateAgentPosition(Vector3 pos) // Just checks for the NavMeshAgent component before setting a position to prevent errors.
    {
        if (_agent)
            _agent.SetDestination(pos);
    }

    private IEnumerator ForcefullyFailInvestigation()
    {
        yield return new WaitForSeconds(10);
        if(_aiController)
            _aiController.UpdateAIState(AIController.AIState.Patrolling);
    }
    public void StopBehaviour()
    {
        _lookAround = false;
        StopAllCoroutines();
    }
}
