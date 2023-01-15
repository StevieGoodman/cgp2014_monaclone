using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class RepBarUpdate : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    [SerializeField] private Slider _pickSlider;
    [SerializeField] private Slider _hackSlider;
    [SerializeField] private Slider _disgSlider;
    [SerializeField] private Slider _knockSlider;

    [SerializeField] private LockPickingAbility _lockpick;
    [SerializeField] private KnockoutAbility _knock;
    // ADD HACK
    // ADD DISGUISE

    public UnityEvent event_test;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    private void OnEnable()
    {
        UpdateAbilityValues();
    }
    /*
     * When the object is enabled the UpdateAbilityValues method fetches all the values for the reputation function
     * There could be ways to improve this method of getting the values but its an easy way to do it for the time being
     */
    private void UpdateAbilityValues()
    {
        float pick_lvl = _player.GetComponent<LockPickingAbility>().reputation;
        _pickSlider.value = pick_lvl / 10;
        Debug.Log(pick_lvl);
        Debug.Log(pick_lvl / 10);

        /* Uncomment when appropriate skill classes have been created
        float hack_lvl = _player.GetComponent<HackAbility>().reputation;
        _pickSlider = hack_lvl / 10;
        */

        float knock_lvl = _player.GetComponent<KnockoutAbility>().reputation;
        _knockSlider.value = knock_lvl / 10;

        /*Uncomment when appropriate skill classes have been created
        float disg_lvl = _player.GetComponent<DisguiseAbility>().reputation;
        _disgSlider.value = disg_lvl / 10; 
         */
    }
}
