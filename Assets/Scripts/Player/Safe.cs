using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Lock))]
public class Safe : MonoBehaviour
{
    // The ability that should receive a bonus.
    public AbilityBonus abilityBonus;

    // The amount that the ability will get.
    public int bonusAmount;
    
    // The lock on the safe.
    private Lock _lock;

    public enum AbilityBonus
    {
        LockPicking,
        KnockOut,
        Hacking,
        Disguise
    }
    private void Awake()
    {
        _lock = GetComponent<Lock>();
        
        // Listen out for the unlockEvent for this safe.
        _lock.whenUnlocked.AddListener(GivePlayerRep);
    }

    private void GivePlayerRep()
    {
        var player = GameManager.Instance.GetPlayerTransform().root.gameObject;

        Ability ability = abilityBonus switch
        {
            AbilityBonus.LockPicking => player.GetComponent<LockPickingAbility>(),
            AbilityBonus.KnockOut => player.GetComponent<KnockoutAbility>(),
            AbilityBonus.Hacking => player.GetComponent<HackAbility>(),
            AbilityBonus.Disguise => player.GetComponent<DisguiseAbility>()
        };
        ability.AlterReputationValue(bonusAmount);
    }
}
