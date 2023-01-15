using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    private GameObject _main_objective;
    private GameObject[] _side_objectives;

    [SerializeField] private TextMeshPro _main_obj_ui;
    [SerializeField] private TextMeshPro _lockpick_side_obj_ui;
    [SerializeField] private TextMeshPro _hack_side_obj_ui;
    [SerializeField] private TextMeshPro _knock_side_obj_ui;
    

    // Start is called before the first frame update
    void Start()
    {
        _main_objective = GameObject.FindGameObjectWithTag("Objective");
        _side_objectives = GameObject.FindGameObjectsWithTag("Safe");
        foreach (GameObject side_objective in _side_objectives) {
            side_objective.GetComponent<Lock>().whenUnlocked.AddListener(
                () => OnUnlock(side_objective.GetComponent<Safe>())
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnUnlock(Safe unlockedSafe)
    {


        switch (unlockedSafe.abilityBonus)
        {


            case Safe.Ability.LockPicking:
                // TEST TEST
                Debug.Log("LOCKPICK TEST");
                break;
            case Safe.Ability.KnockOut:
                Debug.Log("KNOCKOUT TEST");
                break;

            /*
             * 
            
            Safe.Ability.KnockOut => GameManager.Instance.player.GetComponent<LockPickingAbility>(),
        Safe.Ability.Hacking => GameManager.Instance.player.GetComponent<LockPickingAbility>(),
        Safe.Ability.Disguise => GameManager.Instance.player.GetComponent<LockPickingAbility>(),
            
            */
        }


    }
}
