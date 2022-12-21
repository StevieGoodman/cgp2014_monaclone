using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        LockPick,
        Disguise,
        Throwable
    }
    public ItemType itemType;

    public UnityEvent onCollect;
    public UnityEvent onCollectFail;
    // If the player collides with this item. we want to firstly check what type of item we are.
    // Then we want to see if the player can hold another one of those items. If so. We give them it.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var player = GameManager.Instance.player;
        switch (itemType)
        {
            case ItemType.LockPick:
                if (CanBeCollected(player.GetComponentInParent<LockPickingAbility>()))
                {
                    player.GetComponentInParent<LockPickingAbility>().charges++;
                    onCollect?.Invoke();
                    Destroy(gameObject);
                }
                else
                    onCollectFail?.Invoke();
                break;
            case ItemType.Disguise:
                //if (CanBeCollected(player.GetComponent<LockPickingAbility>()))
                    //player.GetComponent<LockPickingAbility>().currentAbilityCharges++;
                break;
            case ItemType.Throwable:
                if (CanBeCollected(player.GetComponentInParent<KnockoutAbility>()))
                {
                    player.GetComponentInParent<KnockoutAbility>().charges++;
                    onCollect?.Invoke();
                    Destroy(gameObject);
                }
                else
                    onCollectFail?.Invoke();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    // This function determines if this item can be collected.
    // It checks each ability level, and on each level, if the current charges matches or goes over
    // the limit. It wont be collected. Otherwise, it will be.
    private bool CanBeCollected(Ability abilityInstance)
    {
        switch (abilityInstance.abilityLevel)
        {
            case Ability.AbilityLevel.Positive:
                if (abilityInstance.charges >= abilityInstance.positiveCharges)
                    return false;
                break;
            case Ability.AbilityLevel.Neutral:
                if (abilityInstance.charges >= abilityInstance.neutralCharges)
                    return false;
                break;
            case Ability.AbilityLevel.Negative:
                if (abilityInstance.charges >= abilityInstance.negativeCharges)
                    return false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        // If we didnt return false on the collection check. we can pick up this item!
        return true;
    }
    
}
