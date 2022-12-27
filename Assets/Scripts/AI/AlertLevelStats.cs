using UnityEngine;

[CreateAssetMenu(menuName = "MonaClone/AlertLevelStats")]
public class AlertLevelStats : ScriptableObject
{
    [Header("AIController Stats")]
    public float investigationTime;
    public float chaseTime;
    public float alertRadius;
    public float minimumChasePeriod;
        
    // Agent Stats
    public float movementSpeed;
    public float rotationSpeed;
        
    // Patrol Stats
    [Header("Sight Stats")]
    // How far around the entity it can see.
    [Range(5, 360)]public float fieldOfView;
    
    // The amount of raycasts used to build the sight mesh.
    [Range(10, 1000)]public int rayCount;
    
    // How far the entity can see.
    [Range(1, 20)]public float viewDistance;

    [Header("Patrol Stats")] 
    public float waitTime;
        
    // Investigation Stats
    [Header("Investigation Stats")]
    public float lookAroundTime;
}
