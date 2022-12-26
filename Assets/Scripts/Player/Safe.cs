using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Lock))]
public class Safe : MonoBehaviour
{
    // The ability that should receive a bonus.
    public Ability abilityBonus;

    // The amount that the ability will get.
    public int bonusAmount;
    
    // The lock on the safe.
    private Lock _lock;

    public enum Ability
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
        switch (abilityBonus)
        {
            case Ability.LockPicking:
                player.GetComponent<LockPickingAbility>().AlterReputationValue(bonusAmount);
                break;
            case Ability.KnockOut:
                player.GetComponent<KnockoutAbility>().AlterReputationValue(bonusAmount);
                break;
            case Ability.Hacking:
                //player.GetComponent<HackAbility>().AlterReputationValue(bonusAmount);
                break;
            case Ability.Disguise:
                //player.GetComponent<DisguiseAbility>().AlterReputationValue(bonusAmount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
