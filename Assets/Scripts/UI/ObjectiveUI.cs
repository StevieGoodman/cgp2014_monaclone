using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectiveUI : MonoBehaviour
{
    private GameObject _main_objective;
    private Safe[] _side_objectives;

    [System.Serializable]
    public struct Objective
    {
        public TextMeshProUGUI GUI;
        public string objectiveDescription;
        public string objectiveCompletion;
    }

    [SerializeField] private GameObject _main_obj_ui;

    public Objective LockpickObjective;
    public Objective HackingObjective;
    public Objective KnockoutObjective;
    public Objective DisguiseObjective;

    // Start is called before the first frame update
    void Start()
    {
        
        // _main_objective = GameObject.FindGameObjectWithTag("Objective");
        _side_objectives = GameObject.FindObjectsOfType<Safe>();
        foreach (var side_objective in _side_objectives) {
            side_objective.GetComponent<Lock>().whenUnlocked.AddListener(() => OnUnlock(side_objective));

            switch (side_objective.abilityBonus)
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
            }
        }
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
        }
    }
}
