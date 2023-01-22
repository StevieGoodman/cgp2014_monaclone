using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using static EnemyState.BehaviourState;

public static class EnemyState
{
    public enum BehaviourState
    {
        Patrolling,
        Investigating,
        Chasing,
        Unconscious
    }
    
    public static Color GetColour(this BehaviourState behaviourState)
    {
        return behaviourState switch
        {
            Patrolling => Color.green,
            Investigating => Color.yellow,
            Chasing => Color.red,
            Unconscious => Color.grey,
            _ => Color.white
        };
    }
}

[RequireComponent(typeof(PatrolBehaviour))] [RequireComponent(typeof(InvestigateBehaviour))] [RequireComponent(typeof(ChaseBehaviour))] [RequireComponent(typeof(UnconsciousBehaviour))]
[RequireComponent(typeof(Sight))] 

public class AIController : MonoBehaviour
{
    [FormerlySerializedAs("state")] [Header("Runtime Values")]
    // The state that the AI is currently in.
    public EnemyState.BehaviourState behaviourState;
    // Has a player been detected at all?
    [FormerlySerializedAs("playerDetected")] public bool targetDetected;

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

    // The radius at which guards can alert each other.
    private float _alertRadius;

    // How long should the ai be forced to chase for?
    private float _minimumChasePeriod;

    // This time ticks up and down depending on player detection.
    [SerializeField]private float _detectionMeter;

    private float DetectionMeter
    {
        get => _detectionMeter;

        set => _detectionMeter = Mathf.Clamp(value, 0, _chaseStartTime + _minimumChasePeriod);
    }

    private const string TargetTag = "Player";

    private void Awake()
    {
        _agent = GetComponentInChildren<NavMeshAgent>();
        _entityBody = _agent.transform;
        
        _patrolBehaviour = GetComponent<PatrolBehaviour>();
        _investigateBehaviour = GetComponent<InvestigateBehaviour>();
        _chaseBehaviour = GetComponent<ChaseBehaviour>();
        _unconsciousBehaviour = GetComponent<UnconsciousBehaviour>();
        
        _sight = GetComponent<Sight>();
        _sight.seenObject.AddListener(CheckForTarget);
        
        // Forces AI into low alert state.
        UpdateAIAlertness(AlertSystem.AlertnessLevel.low);
    }
    private void Start() => UpdateAIState(Patrolling);

    private void Update() => targetDetected = false;
    
    private void FixedUpdate() =>DetectionLogic();

    private void LateUpdate(){ if(!targetDetected) ReduceDetectionMeter();}

    /// <summary>
    /// Updates the state the AI is currently using.
    /// </summary>
    /// <param name="behaviourStateToUpdateTo">What state should the AI become?</param>
    public void UpdateAIState(EnemyState.BehaviourState behaviourStateToUpdateTo)
    {
        if (behaviourState == Unconscious) return;
        behaviourState = behaviourStateToUpdateTo;
        UpdateFieldOfViewColour();
        StopAICoroutines();
        
        EnemyBehaviour behaviour = behaviourState switch
        {
            Patrolling => _patrolBehaviour,
            Investigating => _investigateBehaviour,
            Chasing => _chaseBehaviour,
            Unconscious => _unconsciousBehaviour,
            _ => throw new ArgumentOutOfRangeException()
        };

        if(behaviour == _investigateBehaviour) 
            behaviour.StartBehaviour(positionToInvestigate);
        else
            behaviour.StartBehaviour();
        
        if(behaviour == _chaseBehaviour) DetectionMeter = _chaseStartTime + _minimumChasePeriod;
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
        _sight.SetFieldOfViewColour(behaviourState.GetColour());
        
        if(targetDetected && !GameManager.Instance.player.transform.root.GetComponent<DisguiseAbility>().IsDisguised && behaviourState != Unconscious && behaviourState != Chasing)
            _sight.SetFieldOfViewColour(Color.yellow);
    }

    // Checks if the tag they received is the player.
    private void CheckForTarget(GameObject obj)
    {
        targetDetected = obj.CompareTag(TargetTag);
        
        if (!targetDetected) return;
        if (behaviourState == Unconscious) return;
        IncreaseDetectionMeter(obj);
        UpdateFieldOfViewColour();
    }

    private void IncreaseDetectionMeter(GameObject obj)
    {
        if (obj.transform.root.TryGetComponent<DisguiseAbility>(out var disguiseAbility))
        {
            if(!disguiseAbility.IsDisguised)
                DetectionMeter += Time.deltaTime;
        }
        else
            DetectionMeter += Time.deltaTime;
    }

    private void ReduceDetectionMeter() => DetectionMeter -= Time.deltaTime;

    // Check for player presence and if so. Tick our countdown down.
    private void DetectionLogic()
    {
        if (behaviourState == Unconscious) return;
        // If the player was in the AI's Field of view for long enough. The AI will decide to investigate whatever is going on.
        if (_investigateStartTime < DetectionMeter && DetectionMeter < _chaseStartTime - 0.1 && behaviourState != Investigating)
        {
            _canSendAlerts = true;
            positionToInvestigate = GameManager.Instance.GetPlayerTransform().position;
            UpdateAIState(Investigating);
        }
        // The player has been found! Chase State Should alert people around them and have them chase too!
        if (DetectionMeter >= _chaseStartTime && behaviourState != Chasing)
        {
            if(_canSendAlerts) AlertNearbyGuards();
            UpdateAIState(Chasing);
            _canSendAlerts = false;

            AlertSystem.Instance.Tokens++;
        }
    }
    // Alerts nearby guards to the players presence.
    private void AlertNearbyGuards()
    {
        _canSendAlerts = false;
        
        var guards = Physics2D.CircleCastAll(_entityBody.position, _alertRadius, Vector2.up, _alertRadius, guardLayer);
        foreach (var guard in guards)
        {
            // Have each guard investigate where the player was upon this function being called.
            var aiController = guard.collider.GetComponentInParent<AIController>();
            if (!aiController) continue;
            
            aiController.UpdateAIState(Chasing);
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
        var position = transform.position;
        Handles.DrawWireDisc(position, Vector3.forward, lowAlertStats.alertRadius);
        
        // Shows guards alert radius.
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(position, Vector3.forward, medAlertStats.alertRadius);
        
        // Shows guards alert radius.
        Handles.color = Color.red;
        Handles.DrawWireDisc(position, Vector3.forward, highAlertStats.alertRadius);
    }
    #endif
}
