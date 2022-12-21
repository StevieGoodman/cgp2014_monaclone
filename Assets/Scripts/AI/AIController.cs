using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrolBehaviour))] [RequireComponent(typeof(InvestigateBehaviour))]
[RequireComponent(typeof(Sight))]


public class AIController : MonoBehaviour
{
    [Header("Runtime Values")]
    public AIState aiState;
    public bool playerDetected;

    [Header("Sight Settings")]
    [Tooltip("How long does the player have to be within this AI's view to make them chase.")]
    public float chaseStartTime; 
    
    [Tooltip("How long does the player have to be within this AI's view to make them investigate what they saw?")]
    public float investigateStartTime;

    [Header("AI Agent Settings")] 
    public float movementSpeed;
    public float angularSpeed;

    [Header("Investigation Settings")]
    // This keeps track of the latest investigation position the AI has been assigned.
    [HideInInspector]public Vector3 positionToInvestigate;
    private float _chaseCountdown; // This time ticks up and down depending on player detection.
    
    // COMPONENTS
    private PatrolBehaviour _patrolBehaviour;
    private InvestigateBehaviour _investigateBehaviour;
    private Sight _sight;
    private NavMeshAgent _agent;
    
    public enum AIState
    {
        Patrolling,
        Investigating,
        Chasing, // TODO: Chase state currently not implemented.
        Unconscious // TODO: Unconscious state currently not implemented.
    }

    private void Awake()
    {
        // Assign our required systems.
        AssignAgentValues();
        _patrolBehaviour = GetComponent<PatrolBehaviour>();
        _investigateBehaviour = GetComponent<InvestigateBehaviour>();
        _sight = GetComponent<Sight>();
        _chaseCountdown = chaseStartTime; //Set our internal timer to the max, which is the chase start time. Once this ticks down. the AI will chase the player.
    }

    private void Start()
    {
        UpdateAIState(AIState.Patrolling);
    }

    private void AssignAgentValues()
    {
        _agent = GetComponentInChildren<NavMeshAgent>();
        _agent.speed = movementSpeed;
        _agent.angularSpeed = angularSpeed;
    }
    private void FixedUpdate()
    {
        UpdateFieldOfViewColour();
        DoPlayerDetectionLogic();
    }
    public void UpdateAIState(AIState stateToUpdateTo)
    {
        aiState = stateToUpdateTo;
        switch (aiState)
        {
            case AIState.Patrolling:
                _patrolBehaviour.StopAllCoroutines();
                _investigateBehaviour.StopAllCoroutines();
                _patrolBehaviour.StartPatrolling();
                break;
            case AIState.Investigating:
                _investigateBehaviour.StopAllCoroutines();
                _patrolBehaviour.StopAllCoroutines();
                _investigateBehaviour.GoInvestigatePosition(positionToInvestigate);
                break;
            case AIState.Chasing:
                break;
            case AIState.Unconscious:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    // Updates the FOV cone colour depending on whats going on. TODO: Look into making this more efficient.
    private void UpdateFieldOfViewColour()
    {
        if(playerDetected)
            _sight.SetFieldOfViewColour(Color.yellow);
        else
            _sight.SetFieldOfViewColour(Color.green);
        if (aiState == AIState.Investigating)
            _sight.SetFieldOfViewColour(Color.yellow);
        if (aiState == AIState.Chasing)
            _sight.SetFieldOfViewColour(Color.red);
    }
    
    // Check for player presence and if so. Tick our countdown down.
    private void DoPlayerDetectionLogic()
    {
        if (playerDetected)
        {
            if (_chaseCountdown > 0)
                _chaseCountdown -= Time.fixedDeltaTime;
        }
        else
        {
            if(_chaseCountdown < chaseStartTime)
                _chaseCountdown += Time.fixedDeltaTime;
        }
        // If the player was in the AI's Field of view for long enough. The AI will decide to investigate whatever is going on.
        if (chaseStartTime - investigateStartTime > _chaseCountdown && aiState != AIState.Investigating)
        {
            positionToInvestigate = GameManager.Instance.GetPlayerPosition();
            UpdateAIState(AIState.Investigating);
        }
        // The player has been found! Chase State Should alert people around them and have them chase too!
        if (_chaseCountdown <= 0)
            UpdateAIState(AIState.Chasing);
    }
}
