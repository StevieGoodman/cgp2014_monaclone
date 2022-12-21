using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Throwable : MonoBehaviour
{
    public UnityEvent onCollision;
    private void Start()
    {
        Invoke(nameof(KillMe), 2f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Cause Distraction
        onCollision?.Invoke();
        KillMe();
    }

    // Pushes this object using a direction and a throwing force.
    public void ThrowMe(Vector3 direction, float throwForce) 
    {
        GetComponent<Rigidbody2D>().AddRelativeForce(direction.normalized * throwForce, ForceMode2D.Impulse);
    }

    private void KillMe()
    {

        Destroy(gameObject);
    }
}
