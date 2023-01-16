using System.Collections;
using UnityEditor;
using UnityEngine;

public class LaserController : MonoBehaviour, Hackable
{
    public float alertRadius;
    private bool _laserActive = true;
    
    [SerializeField] private string _targetTag = "Player";
    [SerializeField] private LayerMask _guardLayer = 7;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private LineRenderer _lineRenderer;
    private bool _playerDetected = false;
    private readonly float _detectionCooldown = 5f;
    
    private void FixedUpdate()
    {
        if(_laserActive)
            DrawLaser(_targetTag);
    }

    /// <summary>
    /// Draws a laser from the objects forward position and checks for player presence.
    /// </summary>
    /// <param name="targetTag"> The tag the laser will react to if crossed.</param>
    private void DrawLaser(string targetTag)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 100, _layerMask);
        if (hit)
        {
            _lineRenderer.SetPosition(1, new Vector3(0,hit.distance, 0));
            
            if (hit.collider.CompareTag(targetTag) && !_playerDetected)
            {
                AlertNearbyGuards(transform.position, alertRadius);
                AlertSystem.Instance.Tokens++;
            }
        }
        // If we didnt hit anything, just set the laser line to go 5000 units.
        else
        {
            _lineRenderer.SetPosition(1, transform.up * 5000);
        }
    }
    
    /// <summary>
    /// Forcefully sets all AIController based entities into chase mode.
    /// </summary>
    /// <param name="alertOrigin"> The origin point of the alert.</param>
    /// <param name="alertRangeMeters"> How far the alert can go out in meters.</param>
    private void AlertNearbyGuards(Vector2 alertOrigin, float alertRangeMeters)
    {
        _playerDetected = true;
        Debug.Log("Attempting to alert guards.");
        RaycastHit2D[] guards = Physics2D.CircleCastAll(alertOrigin, alertRadius, Vector2.up, alertRangeMeters, _guardLayer);
        foreach (var guard in guards)
        {
            // Have each guard investigate where the player was upon this function being called.
            var aiController = guard.collider.GetComponentInParent<AIController>();
            if (!aiController) continue;
            aiController.UpdateAIState(AIController.AIState.Chasing);
        }
        if(_playerDetected)
            StartCoroutine(nameof(AlertCooldownCoroutine), _detectionCooldown);
    }

    /// <summary>
    /// Disables the Entity for a specified amount of time.
    /// </summary>
    /// <param name="disableTimeSeconds"> How long in seconds the entity will be disabled.</param>
    public void Hack(float disableTimeSeconds)
    {
        StartCoroutine(nameof(HackDisableCoroutine), disableTimeSeconds);
    }

    private IEnumerator HackDisableCoroutine(float disableTimeSeconds)
    {
        _laserActive = false;
        _lineRenderer.enabled = false;
        yield return new WaitForSeconds(disableTimeSeconds);
        _lineRenderer.enabled = true;
        _laserActive = true;
    }

    private IEnumerator AlertCooldownCoroutine(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        _playerDetected = false;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Shows alert radius.
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.forward, alertRadius);
        Debug.DrawRay(transform.position, transform.up * 5000, Color.red);
    }
#endif
}
