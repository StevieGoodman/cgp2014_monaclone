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
        GameObject player = GameManager.Instance.player;
        Ability abilityComponent = itemType switch
        {
            ItemType.LockPick => player.GetComponentInParent<LockPickingAbility>(),
            ItemType.Throwable => player.GetComponentInParent<KnockoutAbility>(),
            ItemType.Disguise => player.GetComponentInParent<DisguiseAbility>(),
            _ => throw new ArgumentOutOfRangeException()
        };
        abilityComponent.Charges++;
        onCollect?.Invoke();
        Destroy(gameObject);
    }
}
