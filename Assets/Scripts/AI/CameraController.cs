using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PatrolBehaviour))] [RequireComponent(typeof(Sight))]

// The cameras are a unique type of entity. So they have their own controller.
public class CameraController : MonoBehaviour, Hackable
{
    public AIState aiState;

    public float alertStartTime;

    public float alertRadius;
    // Is the player within the Cameras view?
    private bool _playerDetected;

    // use patrol behaviour to have the camera look around.
    private PatrolBehaviour _patrolBehaviour;

    // Used to disable the camera.
    private UnconsciousBehaviour _unconsciousBehaviour;

    // DO NOT CHANGE LAYER ORDER OR THIS WILL BREAK!
    [SerializeField] private LayerMask guardLayer = 7;

    // Camera needs to see.
    private Sight _sight;
    private Rigidbody2D _entityBody;
    private float _detectionMeter;
    private bool _canSendAlerts;
    

    public enum AIState
    {
        Patrolling,
        Unconscious
    }

    private void Awake()
    {
        _patrolBehaviour = GetComponent<PatrolBehaviour>();
        _entityBody = GetComponentInChildren<Rigidbody2D>();
        _unconsciousBehaviour = GetComponent<UnconsciousBehaviour>();
        _sight = GetComponent<Sight>();
        _sight.seenTag.AddListener(CheckForPlayer);
    }
    private void Start()
    {
        UpdateAIState(AIState.Patrolling);
    }
    private void Update()
    {
        // Se player detected to false, in late update on sight, if the player was seen.
        // It will tell us. Otherwise this wont change.
        _playerDetected = false;
        
    }
    private void FixedUpdate()
    {
        DetectionLogic();

        // Enable detection if its disabled and the detection timer is back to full.
        if (!_canSendAlerts && _detectionMeter == alertStartTime)
            _canSendAlerts = true;
    }

    public void UpdateAIState(AIState stateToUpdateTo)
    {
        Debug.Log("AIState: " + stateToUpdateTo);
        if (aiState == AIState.Unconscious) _unconsciousBehaviour.GainConsciousness();
        aiState = stateToUpdateTo;
        switch (aiState)
        {
            case AIState.Patrolling:
                StopAICoroutines();
                _patrolBehaviour.StartPatrolling();
                break;
            case AIState.Unconscious:
                StopAICoroutines();
                _unconsciousBehaviour.LoseConsciousness();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DetectionLogic()
    {
        if (aiState == AIState.Unconscious)
        {
            _sight.SetFieldOfViewColour(Color.gray);
            return;
        }
        // If we detect the player, tick down the detection timer.
        if (_playerDetected)
        {
            _sight.SetFieldOfViewColour(Color.yellow);
            _detectionMeter -= Time.fixedDeltaTime;
        }
        // If we dont find them. bring it back up.
        else
        {
            _sight.SetFieldOfViewColour(Color.green);
            _detectionMeter += Time.fixedDeltaTime;

            if (_detectionMeter > alertStartTime)
                _detectionMeter = alertStartTime;
        }

        if (_detectionMeter <= 0)
        {
            _sight.SetFieldOfViewColour(Color.red);
            if(_canSendAlerts)
                AlertNearbyGuards();
        }
    }

    private void AlertNearbyGuards()
    {
        _canSendAlerts = false;
        Debug.Log("Attempting to alert guards.");
        RaycastHit2D[] guards = Physics2D.CircleCastAll(_entityBody.position, alertRadius, Vector2.up, alertRadius, guardLayer);
        foreach (var guard in guards)
        {

            // Have each guard investigate where the player was upon this function being called.
            var aiController = guard.collider.GetComponentInParent<AIController>();
            if (!aiController) continue;
            
            //aiController.positionToInvestigate = GameManager.Instance.GetPlayerPosition();
            //aiController.UpdateAIState(AIController.AIState.Investigating);
            aiController.UpdateAIState(AIController.AIState.Chasing);
            Debug.Log("Guard Alerted.");
        }
    }
    private void CheckForPlayer(string tag)
    {
        if (tag == "Player")
            _playerDetected = true;
    }
    
    private void StopAICoroutines()
    {
        _patrolBehaviour.StopAllCoroutines();
    }

    public void Hack(float timeInSeconds)
    {
        UpdateAIState(AIState.Unconscious);
        StartCoroutine(Wait(timeInSeconds));
        IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            UpdateAIState(AIState.Patrolling);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Shows alert radius.
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.forward, alertRadius);
    }
    #endif
}
