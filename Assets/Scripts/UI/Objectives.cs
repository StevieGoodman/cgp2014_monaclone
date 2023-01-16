using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    private GameObject _main_objective;
    private GameObject[] _side_objectives;

    [SerializeField] private GameObject _main_obj_ui;

    [SerializeField] private GameObject _lockpick_side_obj_ui;
    [SerializeField] private GameObject _hack_side_obj_ui;
    [SerializeField] private GameObject _knock_side_obj_ui;
    [SerializeField] private GameObject _disg_side_obj_ui;
    

    // Start is called before the first frame update
    void Start()
    {
        
        // _main_objective = GameObject.FindGameObjectWithTag("Objective");
        _side_objectives = GameObject.FindGameObjectsWithTag("Safe");
        foreach (GameObject side_objective in _side_objectives) {
            Safe safe_obj = side_objective.GetComponent<Safe>();
            side_objective.GetComponent<Lock>().whenUnlocked.AddListener(
                () => OnUnlock(safe_obj));
            Debug.Log("Objective Located");
            switch (safe_obj.abilityBonus)
            {
                case Safe.Ability.LockPicking:
                    Debug.Log("LockPick Side Obj located");
                    _lockpick_side_obj_ui.GetComponent<TextMeshPro>().text = "The Reporter's Juicy News";
                    break;
                case Safe.Ability.KnockOut:
                    _knock_side_obj_ui.GetComponent<TextMeshPro>().text = "The Cleaner's Dirty Laundry";
                    break;
                case Safe.Ability.Disguise:
                    _disg_side_obj_ui.GetComponent<TextMeshPro>().text = "The Gentleman's Blackmail";
                    break;
                case Safe.Ability.Hacking:
                    _hack_side_obj_ui.GetComponent<TextMeshPro>().text = "The Hacker's Incriminating Data";
                    break;
            }
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
                _lockpick_side_obj_ui.GetComponent<TextMeshPro>().color = Color.green;
                _lockpick_side_obj_ui.GetComponent<TextMeshPro>().text = "The Reporter's Juicy News - Reported!";
                break;
            case Safe.Ability.KnockOut:
                _knock_side_obj_ui.GetComponent<TextMeshPro>().color = Color.green;
                _knock_side_obj_ui.GetComponent<TextMeshPro>().text = "The Cleaner's Dirty Laundry - Aired Out!";
                break;
            case Safe.Ability.Disguise:
                _disg_side_obj_ui.GetComponent<TextMeshPro>().color = Color.green;
                _disg_side_obj_ui.GetComponent<TextMeshPro>().text = "The Gentleman's Blackmail - Mailed!";
                break;
            case Safe.Ability.Hacking:
                _hack_side_obj_ui.GetComponent<TextMeshPro>().color = Color.green;
                _disg_side_obj_ui.GetComponent<TextMeshPro>().text = "The Hacker's Incriminating Data - Cracked!";
                break;
        }


    }
}
