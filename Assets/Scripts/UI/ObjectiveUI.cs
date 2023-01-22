using System;
using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    [System.Serializable]
    public struct Objective
    {
        public TextMeshProUGUI gui;
        public string objectiveDescription;
        public string objectiveCompletion;
    }

    public Objective mainObjective;
    public Objective lockpickObjective;
    public Objective hackingObjective;
    public Objective knockoutObjective;
    public Objective disguiseObjective;
    
    private void Start() => InitializeUIElements();
    
    private void InitializeUIElements()
    {
        var sideObjectives = FindObjectsOfType<Safe>();
        foreach (var objective in sideObjectives) {
            objective.GetComponent<Lock>().whenUnlocked.AddListener(() => OnUnlock(objective));

            var obj = objective.abilityBonus switch
            {
                Safe.AbilityBonus.LockPicking => lockpickObjective,
                Safe.AbilityBonus.KnockOut => knockoutObjective,
                Safe.AbilityBonus.Hacking => hackingObjective,
                Safe.AbilityBonus.Disguise => disguiseObjective,
                _ => throw new ArgumentOutOfRangeException()
            };
            obj.gui.text = obj.objectiveDescription;
        }
        
        //Main Objective
        var mainObjective = FindObjectOfType<MainObjective>();
        if (!mainObjective) return;
        mainObjective.onCollected.AddListener(OnMainObjectiveUnlock);

        this.mainObjective.gui.text = this.mainObjective.objectiveDescription;
    }

    private void OnUnlock(Safe unlockedSafe)
    {
        var obj = unlockedSafe.abilityBonus switch
        {
            Safe.AbilityBonus.LockPicking => lockpickObjective,
            Safe.AbilityBonus.KnockOut => knockoutObjective,
            Safe.AbilityBonus.Hacking => hackingObjective,
            Safe.AbilityBonus.Disguise => disguiseObjective,
            _ => throw new ArgumentOutOfRangeException()
        };
        obj.gui.text = obj.objectiveCompletion;
        obj.gui.color = Color.green;
    }

    private void OnMainObjectiveUnlock()
    {
        mainObjective.gui.text = mainObjective.objectiveCompletion;
        mainObjective.gui.color = Color.green;
    }
}
