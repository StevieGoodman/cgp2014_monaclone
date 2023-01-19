using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectiveUI : MonoBehaviour
{

    [System.Serializable]
    public struct Objective
    {
        public TextMeshProUGUI GUI;
        public string objectiveDescription;
        public string objectiveCompletion;
    }

    public Objective MainObjective;
    public Objective LockpickObjective;
    public Objective HackingObjective;
    public Objective KnockoutObjective;
    public Objective DisguiseObjective;
    
    private void Start() => InitializeUIElements();
    
    private void InitializeUIElements()
    {
        var sideObjectives = FindObjectsOfType<Safe>();
        foreach (var objective in sideObjectives) {
            objective.GetComponent<Lock>().whenUnlocked.AddListener(() => OnUnlock(objective));

            switch (objective.abilityBonus)
            {
                case Safe.AbilityBonus.LockPicking:
                    LockpickObjective.GUI.text = LockpickObjective.objectiveDescription;
                    break;
                case Safe.AbilityBonus.KnockOut:
                    KnockoutObjective.GUI.text = KnockoutObjective.objectiveDescription;
                    break;
                case Safe.AbilityBonus.Disguise:
                    DisguiseObjective.GUI.text = DisguiseObjective.objectiveDescription;
                    break;
                case Safe.AbilityBonus.Hacking:
                    HackingObjective.GUI.text = HackingObjective.objectiveDescription;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        //Main Objective
        var mainObjective = FindObjectOfType<MainObjective>();
        if (!mainObjective) return;
        mainObjective.onCollected.AddListener(OnMainObjectiveUnlock);

        MainObjective.GUI.text = MainObjective.objectiveDescription;
    }

    private void OnUnlock(Safe unlockedSafe)
    {
        switch (unlockedSafe.abilityBonus)
        {
            case Safe.AbilityBonus.LockPicking:
                LockpickObjective.GUI.text = LockpickObjective.objectiveCompletion;
                LockpickObjective.GUI.color = Color.green;
                break;
            case Safe.AbilityBonus.KnockOut:
                KnockoutObjective.GUI.text = KnockoutObjective.objectiveCompletion;
                KnockoutObjective.GUI.color = Color.green;
                break;
            case Safe.AbilityBonus.Disguise:
                DisguiseObjective.GUI.text = DisguiseObjective.objectiveCompletion;
                DisguiseObjective.GUI.color = Color.green;
                break;
            case Safe.AbilityBonus.Hacking:
                HackingObjective.GUI.text = HackingObjective.objectiveCompletion;
                HackingObjective.GUI.color = Color.green;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnMainObjectiveUnlock()
    {
        MainObjective.GUI.text = MainObjective.objectiveCompletion;
        MainObjective.GUI.color = Color.green;
    }
}
