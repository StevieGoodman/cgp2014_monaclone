using UnityEngine;
using UnityEngine.Events;

public class MainObjective : MonoBehaviour
{
    public UnityEvent onCollected;
    private void OnTriggerEnter2D(Collider2D col)
    {
        onCollected?.Invoke();
        
        // Janky.
        var exit = FindObjectOfType<LevelExit>().GetComponent<Collider2D>().enabled = true;
        
        Destroy(gameObject);
    }
}
