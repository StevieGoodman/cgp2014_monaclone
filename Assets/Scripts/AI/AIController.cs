using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrolBehaviour))] [RequireComponent(typeof(InvestigateBehaviour))] [RequireComponent(typeof(ChaseBehaviour))] [RequireComponent(typeof(UnconsciousBehaviour))]
[RequireComponent(typeof(Sight))] 

// ABOUT //
/*
 *  The AI Controller drives and determines what behaviour the entity should be in.
 *  There are four states:
 *  - Patrolling
 *  - Investigating
 *  - Chasing
 *  - Unconscious
 *
 *  Each state is driven by its own component, which works independently from the AIController.
 *
 *  Patrolling - This behaviour takes input nodes, and follows them one by one.
 *  pretty simple stuff.
 * 
 *  Investigating - If this behaviour is called, it will take whatever is in the positionToInvestigate
 *  value and will then investigate it, they first go to the position, then choose two random rotations
 *  to look in, if they do not change state in that time, they fail the investigation and return to patrolling.
 *
 *  Chasing - When chasing, the entity will constantly follow the player.
 *
 *  Unconscious - The entity is still, not doing anything.
 *
 *  In this controller, a timer called the detection meter, starts at a certain value, if the timer ticks down
 *  below a certain threshold set by the investigateStartTime value, it will switch to the investigate behaviour.
 *  other things such as a throwable can force a switch to investigation from Patrolling. If the player is within
 *  the entity's view for long enough, Chase mode activates. at that point, the player will have to stay out of the
 *  entity's view so the detection timer ticks back up back to investigation, if an investigation fails, they will
 *  return to patrolling again.
 * 
*/

public class AIController : MonoBehaviour
{
    [Header("Runtime Values")]
    // The state that the AI is currently in.
    public AIState aiState;
    // Has a player been detected at all?
    public bool playerDetected;

    
    [Header("Sight Settings")]
    //How long does the player have to be within this AI's view to make them investigate what they saw
    public float investigateStartTime;
    
    //How long does the player have to be within this AI's view to make them chase.
    public float chaseStartTime;

    // The radius at which guards can alert eachother.
    public float alertRadius;

    // This time ticks up and down depending on player detection.
    public float detectionMeter;

    // The layer that guards are on. Should be BehindDarkness.
    public LayerMask guardLayer;
    
    
    [Header("AI Settings")] 
    // How fast can the AI move?
    public float movementSpeed;
    
    // How fast can the AI rotate?
    public float angularSpeed;
    
    // This keeps track of the latest investigation position the AI has been assigned.
    [HideInInspector]public Vector3 positionToInvestigate;
    
    // COMPONENTS
    private PatrolBehaviour _patrolBehaviour;
    private InvestigateBehaviour _investigateBehaviour;
    private ChaseBehaviour _chaseBehaviour;
    private UnconsciousBehaviour _unconsciousBehaviour;
    private Sight _sight;
    private NavMeshAgent _agent;
    private Transform _entityBody;
    private bool _canSendAlerts;
    
    public enum AIState
    {
        Patrolling,
        Investigating,
        Chasing,
        Unconscious
    }

    private void Awake()
    {
        _agent = GetComponentInChildren<NavMeshAgent>();
        _entityBody = _agent.transform;
        _patrolBehaviour = GetComponent<PatrolBehaviour>();
        _investigateBehaviour = GetComponent<InvestigateBehaviour>();
        _chaseBehaviour = GetComponent<ChaseBehaviour>();
        _unconsciousBehaviour = GetComponent<UnconsciousBehaviour>();
        _sight = GetComponent<Sight>();
        _sight.seenTag.AddListener(PlayerDetected);
        detectionMeter = chaseStartTime; //Set our internal timer to the max, which is the chase start time. Once this ticks down. the AI will chase the player.
    }
    private void Start()
    {
        UpdateAIState(AIState.Patrolling);
    }

    private void Update()
    {
        playerDetected = false;
    }

    private void FixedUpdate()
    {
        UpdateFieldOfViewColour();
        
        DetectionLogic();
    }

    private void AssignAgentValues(float move, float angular)
    {
        if (!_agent) return;
        
        _agent.speed = move;
        _agent.angularSpeed = angular;

    }
    public void UpdateAIState(AIState stateToUpdateTo)
    {
        Debug.Log("AIState: " + stateToUpdateTo);
        if (aiState == AIState.Unconscious) return;
        aiState = stateToUpdateTo;
        switch (aiState)
        {
            case AIState.Patrolling:
                StopAICoroutines();
                AssignAgentValues(movementSpeed, angularSpeed);
                _patrolBehaviour.StartPatrolling();
                break;
            case AIState.Investigating:
                StopAICoroutines();
                AssignAgentValues(movementSpeed, angularSpeed);
                _investigateBehaviour.GoInvestigatePosition(positionToInvestigate);
                break;
            case AIState.Chasing:
                StopAICoroutines();
                AssignAgentValues(movementSpeed, angularSpeed * 3);
                detectionMeter = -3; // Sets the timer to a negative value to make it harder for the AI to chase for longer.
                _chaseBehaviour.StartChasing();
                break;
            case AIState.Unconscious:
                StopAICoroutines();
                _unconsciousBehaviour.LoseConsciousness();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    // Updates the FOV cone colour depending on whats going on.
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
        if(aiState == AIState.Unconscious)
            _sight.SetFieldOfViewColour(Color.gray);
    }

    private void PlayerDetected(string tag)
    {
        Debug.Log("Tag seen: " + tag);
        if (tag == "Player")
            playerDetected = true;
    }
    // Check for player presence and if so. Tick our countdown down.
    private void DetectionLogic()
    {
        if (aiState == AIState.Unconscious) return;
        // If we detect the player, tick down the detection timer.
        if (playerDetected)
            detectionMeter -= Time.fixedDeltaTime;
        // If we dont find them. bring it back up.
        else
        {
            detectionMeter += Time.fixedDeltaTime;
            if (detectionMeter > chaseStartTime)
                detectionMeter = chaseStartTime;
        }
        // If the player was in the AI's Field of view for long enough. The AI will decide to investigate whatever is going on.
        if (chaseStartTime - investigateStartTime > detectionMeter && detectionMeter > 0.1 && aiState != AIState.Investigating)
        {
            _canSendAlerts = true;
            positionToInvestigate = GameManager.Instance.GetPlayerPosition();
            UpdateAIState(AIState.Investigating);
        }
        // The player has been found! Chase State Should alert people around them and have them chase too!
        if (detectionMeter <= 0 && aiState != AIState.Chasing)
        {
            if(_canSendAlerts)
                AlertNearbyGuards();
            
            UpdateAIState(AIState.Chasing);
            _canSendAlerts = false;
        }
    }
    
    private void AlertNearbyGuards()
    {
        _canSendAlerts = false;
        Debug.Log("Attempting to alert nearby guards.");
        RaycastHit2D[] guards = Physics2D.CircleCastAll(_entityBody.position, alertRadius, Vector2.up, alertRadius, guardLayer);
        foreach (var guard in guards)
        {

            // Have each guard investigate where the player was upon this function being called.
            var aiController = guard.collider.GetComponentInParent<AIController>();
            if (!aiController) continue;
            
            aiController.UpdateAIState(AIState.Chasing);
            Debug.Log("Guard Alerted.");
        }
    }
    private void StopAICoroutines()
    {
        _patrolBehaviour.StopBehaviour();
        _investigateBehaviour.StopBehaviour();
        _chaseBehaviour.StopBehaviour();
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Shows guards alert radius.
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.forward, alertRadius);
    }
    #endif
}
