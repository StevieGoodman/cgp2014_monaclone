using System;
using UnityEngine;
using UnityEngine.Events;


public class Lock : MonoBehaviour
{ 
    public bool locked; // Used to prevent any code from the unlock being called twice if already unlocked.
    
    public UnityEvent whenUnlocked; // Event that people can use in the inspector to attach extra functions and code.
    
    public void Unlock() // This function calls anything attached to the unlock event.
    {
        if (!locked) return;
        // If the door is locked. Unlock it and invoke the unlock event.
        locked = false;
        whenUnlocked?.Invoke();
    }
}
