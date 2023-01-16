using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class RepBarUpdate : MonoBehaviour
{
    private GameObject _player;

    [SerializeField] private Slider _pickSlider;
    [SerializeField] private Slider _hackSlider;
    [SerializeField] private Slider _disgSlider;
    [SerializeField] private Slider _knockSlider;

    private LockPickingAbility _lockpick;
    private KnockoutAbility _knock;
    // ADD HACK
    // ADD DISGUISE

    private void Awake()
    {
        _player = transform.root.gameObject;
        _lockpick = _player.GetComponent<LockPickingAbility>();
        _knock =  _player.GetComponent<KnockoutAbility>();
        //_hack = _player.GetComponent<HackAbility>();
        //_disguise = _player.GetComponent<DisguiseAbility>();
        
        // Add listeners for each bar.
        _lockpick.reputationValueAltered.AddListener(() => UpdateAbilityValues(_lockpick, _pickSlider));
        _knock.reputationValueAltered.AddListener(() => UpdateAbilityValues(_knock, _knockSlider));
        //_hack.reputationValueAltered.AddListener(() => UpdateAbilityValues(_hack, _hackSlider));
        //_disguise.reputationValueAltered.AddListener(() => UpdateAbilityValues(_disguise, _disgSlider));
    }

    private void Start()
    {
        UpdateAbilityValues(_lockpick, _pickSlider);
        UpdateAbilityValues(_knock, _knockSlider);
        //UpdateAbilityValues(_hack, _hackSlider);
        //UpdateAbilityValues(_disguise, _disgSlider);
    }
    /*
     * When the object is enabled the UpdateAbilityValues method fetches all the values for the reputation function
     * There could be ways to improve this method of getting the values but its an easy way to do it for the time being
     */
    
    /// <summary>
    /// Updates a Ability bar, using the specified ability and slider.
    /// </summary>
    /// <param name="ability">The ability the reputation value will be taken from</param>
    /// <param name="sliderToUpdate">The bar to update</param>
    private void UpdateAbilityValues(Ability ability, Slider sliderToUpdate) => sliderToUpdate.value = ability.Reputation / 10;
}
