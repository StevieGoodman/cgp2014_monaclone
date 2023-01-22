using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class InvestigateBehaviour : EnemyBehaviour
{
    // How long should the AI investigate a location for before going back to other tasks?
    public float lookAroundTime;
    
    // Investigation start and end events
    public UnityEvent onInvestigationStart;
    public UnityEvent onInvestigationEnd;
    
    private AIController _aiController;
    private bool _lookAround;
    private Quaternion _randomRot;
    
    protected override void GetComponents()
    {
        base.GetComponents();
        _aiController = GetComponent<AIController>();
    }
    
    private void Update()
    {
        if(_lookAround)  Agent.transform.rotation = Quaternion.RotateTowards(Agent.transform.rotation, _randomRot, Time.deltaTime * Agent.angularSpeed);
    }

    public override void StartBehaviour(Vector3 posToInvestigate = default) => StartCoroutine(nameof(InvestigatePosition), posToInvestigate);
    
    private IEnumerator InvestigatePosition(Vector3 posToInvestigate)
    {
        UpdateAgentPosition(Agent.transform.position);
        
        // Call investigation start event.
        onInvestigationStart?.Invoke();
        
        yield return new WaitForSeconds(.2f);
        UpdateAgentPosition(posToInvestigate);
        
        // Fixes an issue where the wait until condition was immediately satisfied.
        yield return new WaitForSeconds(0.1f);
        
        // Forcefully ends the investigation if the condition isn't met in 10 seconds.
        StartCoroutine(ForcefullyFailInvestigation());
        
        yield return new WaitUntil(() => Agent.remainingDistance < 0.1f);
        
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
    private void UpdateAgentPosition(Vector3 pos) => Agent.SetDestination(pos);

    private IEnumerator ForcefullyFailInvestigation()
    {
        yield return new WaitForSeconds(10);
        if(_aiController) _aiController.UpdateAIState(AIController.AIState.Patrolling);
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
        _lookAround = false;
    }
}
