using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Level Settings")]
    // How much time is remaining in the level?
    public float timeRemaining;
    
    [Header("Runtime Values")]
    // The players game-object, all scripts can reference it through here.
    public GameObject player;

    // Holds an array of all AIControllers in the scene.
    public AIController[] aiControllers;
    
    
    [Header("Editor Values")]
    // Used in the editor to clear darkness.
    public GameObject darknessClearer;
    
    
    // Singleton instance of the GameManager.
    public static GameManager Instance;
    
    private void Awake()
    {
        Instance = this;
        player = GameObject.FindWithTag("Player");
        
        darknessClearer.SetActive(false);

        // Get all the AIControllers in this scene and store them for use.
        aiControllers = FindObjectsOfType<AIController>();
    }
    public Transform GetPlayerTransform() {return player.transform;}

    private void OnDrawGizmos()
    {
        // If we have a darkness clearer and we are not in playmode. activate the clearer.
        if (!Application.isPlaying && darknessClearer)
            darknessClearer.SetActive(true);
    }
}
