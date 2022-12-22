using UnityEngine;
public class GameManager : MonoBehaviour
{

    public GameObject player;
    public float timeRemaining;
    [Space]
    public static GameManager Instance;
    
    private void Awake()
    {
        Instance = this;
        player = GameObject.FindWithTag("Player");
    }
    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }
    public Transform GetPlayerTransform()
    {
        return player.transform;
    }
}
