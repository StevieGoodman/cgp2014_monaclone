using System;
using UnityEngine;
using UnityEngine.Events;


public class Lock : MonoBehaviour
{ 
    // Used to prevent any code from the unlock being called twice if already unlocked.
    public bool locked = true;

    // How long does it take to unlock this safe?
    public float unlockTime;
    
    // Event that people can use in the inspector to attach extra functions and code.
    public UnityEvent whenUnlocked;
    
    public void Unlock() // This function calls anything attached to the unlock event.
    {
        if (!locked) return;
        // If the door is locked. Unlock it and invoke the unlock event.
        locked = false;
        whenUnlocked?.Invoke();
    }
}
