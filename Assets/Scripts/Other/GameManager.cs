using UnityEngine;
public class GameManager : MonoBehaviour
{

    public GameObject player;
    public float timeRemaining;
    [Space]
    public static GameManager Instance;

    // Used in the editor to clear darkness.
    public GameObject darknessClearer;
    
    private void Awake()
    {
        Instance = this;
        player = GameObject.FindWithTag("Player");
        darknessClearer.SetActive(false);
    }
    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }
    public Transform GetPlayerTransform()
    {
        return player.transform;
    }

    private void OnDrawGizmos()
    {
        // If we have a darkness clearer and we are not in playmode. activate the clearer.
        if (!Application.isPlaying && darknessClearer)
            darknessClearer.SetActive(true);
    }
}
