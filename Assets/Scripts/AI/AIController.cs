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
    
    // The layer that guards are on. Should be BehindDarkness.
    public LayerMask guardLayer;
    
    [Header("AI Settings")]
    public AlertLevelStats lowAlertStats;
    public AlertLevelStats medAlertStats;
    public AlertLevelStats highAlertStats;

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
    
    //How long does the player have to be within this AI's view to make them investigate what they saw
    private float _investigateStartTime;
    
    //How long does the player have to be within this AI's view to make them chase.
    private float _chaseStartTime;

    // The radius at which guards can alert eachother.
    private float _alertRadius;

    // How long should the ai be forced to chase for?
    private float _minimumChasePeriod;

    // This time ticks up and down depending on player detection.
    private float _detectionMeter;
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
        
        // Forces AI into low alert state.
        UpdateAIAlertness(AlertSystem.AlertnessLevel.low);
        _detectionMeter = lowAlertStats.chaseTime;
    }
    private void Start()
    {
        // Sets the AI to start patrolling.
        UpdateAIState(AIState.Patrolling);
    }
    private void Update()
    {
        // Sets the player to be not detected. In the sight component.
        // On late update it will alter this value if the player is within our view.
        playerDetected = false;
    }
    private void FixedUpdate()
    {
        UpdateFieldOfViewColour();
        DetectionLogic();
    }
    // Updates the behaviour the AI is using.
    public void UpdateAIState(AIState stateToUpdateTo)
    {
        Debug.Log("AIState: " + stateToUpdateTo);
        if (aiState == AIState.Unconscious) return;
        aiState = stateToUpdateTo;
        StopAICoroutines();
        switch (aiState)
        {
            case AIState.Patrolling:
                _patrolBehaviour.StartPatrolling();
                break;
            case AIState.Investigating:
                _investigateBehaviour.GoInvestigatePosition(positionToInvestigate);
                break;
            case AIState.Chasing:
                _detectionMeter = -_minimumChasePeriod;
                _chaseBehaviour.StartChasing();
                break;
            case AIState.Unconscious:
                _unconsciousBehaviour.LoseConciousness();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    // Updates the alertness of the AI.
    public void UpdateAIAlertness(AlertSystem.AlertnessLevel alertnessLevel)
    {
        var stats = alertnessLevel switch
        {
            AlertSystem.AlertnessLevel.low => lowAlertStats,
            AlertSystem.AlertnessLevel.medium => medAlertStats,
            AlertSystem.AlertnessLevel.high => highAlertStats,
            _ => throw new ArgumentOutOfRangeException(nameof(alertnessLevel), alertnessLevel, null)
        };
        ApplyAlertStats(stats);
    }
    // Applies the stats of the AI.
    private void ApplyAlertStats(AlertLevelStats stats)
    {
        // Apply AI Controller Stats
        _investigateStartTime = stats.investigationTime;
        _chaseStartTime = stats.chaseTime;
        _minimumChasePeriod = stats.minimumChasePeriod;
        
        // Apply Agent Stats
        _agent.speed = stats.movementSpeed;
        _agent.angularSpeed = stats.rotationSpeed;
        // Apply Patrol Stats
        _patrolBehaviour.waitTime = stats.waitTime;
        
        // Apply Investigation Stats
        _investigateBehaviour.lookAroundTime = stats.lookAroundTime;
        
        // Apply Sight Stats
        _sight.fieldOfView = stats.fieldOfView;
        _sight.rayCount = stats.rayCount;
        _sight.viewDistance = stats.viewDistance;
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
    
    // Checks if the tag they received is the player.
    private void PlayerDetected(string tag){ playerDetected = tag == "Player";}

    // Check for player presence and if so. Tick our countdown down.
    private void DetectionLogic()
    {
        if (aiState == AIState.Unconscious) return;
        // If we detect the player, tick down the detection timer.
        if (playerDetected)
            _detectionMeter -= Time.fixedDeltaTime;
        // If we dont find them. bring it back up.
        else
        {
            _detectionMeter += Time.fixedDeltaTime;
            if (_detectionMeter > _chaseStartTime)
                _detectionMeter = _chaseStartTime;
        }
        // If the player was in the AI's Field of view for long enough. The AI will decide to investigate whatever is going on.
        if (_chaseStartTime - _investigateStartTime > _detectionMeter && _detectionMeter > 0.1 && aiState != AIState.Investigating)
        {
            _canSendAlerts = true;
            positionToInvestigate = GameManager.Instance.GetPlayerTransform().position;
            UpdateAIState(AIState.Investigating);
        }
        // The player has been found! Chase State Should alert people around them and have them chase too!
        if (_detectionMeter <= 0 && aiState != AIState.Chasing)
        {
            if(_canSendAlerts) AlertNearbyGuards();
            UpdateAIState(AIState.Chasing);
            _canSendAlerts = false;

            AlertSystem.Instance.Tokens++;
        }
    }
    // Alerts nearby guards to the players presence.
    private void AlertNearbyGuards()
    {
        _canSendAlerts = false;
        RaycastHit2D[] guards = Physics2D.CircleCastAll(_entityBody.position, _alertRadius, Vector2.up, _alertRadius, guardLayer);
        foreach (var guard in guards)
        {
            // Have each guard investigate where the player was upon this function being called.
            var aiController = guard.collider.GetComponentInParent<AIController>();
            if (!aiController) continue;
            
            aiController.UpdateAIState(AIState.Chasing);
        }
    }
    // Stops all AI behaviour.
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
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, Vector3.forward, lowAlertStats.alertRadius);
        
        // Shows guards alert radius.
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.forward, medAlertStats.alertRadius);
        
        // Shows guards alert radius.
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.forward, highAlertStats.alertRadius);
    }
    #endif
}
