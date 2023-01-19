using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AbilityUI : MonoBehaviour
{
    private GameObject _player;

    [SerializeField] private Slider _pickSlider;
    [SerializeField] private Slider _hackSlider;
    [SerializeField] private Slider _disgSlider;
    [SerializeField] private Slider _knockSlider;

    [Space]
    [SerializeField]private TextMeshProUGUI _pickCount;
    [SerializeField]private TextMeshProUGUI _hackCount;
    [SerializeField]private TextMeshProUGUI _disgCount;
    [SerializeField]private TextMeshProUGUI _knockCount;

    [Space]
    [SerializeField]private LockPickingAbility _lockpick;
    [SerializeField]private KnockoutAbility _knock;
    [SerializeField]private HackAbility _hack;
    [SerializeField]private DisguiseAbility _disguise;

    private void Start()
    {
        _player = GameManager.Instance.GetPlayerTransform().root.gameObject;
        _lockpick = _player.GetComponent<LockPickingAbility>();
        _knock =  _player.GetComponent<KnockoutAbility>();
        _hack = _player.GetComponent<HackAbility>();
        _disguise = _player.GetComponent<DisguiseAbility>();
        
        // Add listeners for each bar.
        _lockpick.reputationValueAltered.AddListener(() => UpdateReputationBar(_lockpick, _pickSlider));
        _knock.reputationValueAltered.AddListener(() => UpdateReputationBar(_knock, _knockSlider));
        _hack.reputationValueAltered.AddListener(() => UpdateReputationBar(_hack, _hackSlider));
        _disguise.reputationValueAltered.AddListener(() => UpdateReputationBar(_disguise, _disgSlider));
        
        _lockpick.onAbilityUsed.AddListener(() => UpdateAbilityCharges(_lockpick, _pickCount));
        _knock.onAbilityUsed.AddListener(() => UpdateAbilityCharges(_knock, _knockCount));
        _hack.onAbilityUsed.AddListener(() => UpdateAbilityCharges(_hack, _hackCount));
        _disguise.onAbilityUsed.AddListener(() => UpdateAbilityCharges(_disguise, _disgCount));
        
        UpdateReputationBar(_lockpick, _pickSlider);
        UpdateReputationBar(_knock, _knockSlider);
        UpdateReputationBar(_hack, _hackSlider);
        UpdateReputationBar(_disguise, _disgSlider);
        
        UpdateAbilityCharges(_lockpick, _pickCount);
        UpdateAbilityCharges(_knock, _knockCount);
        UpdateAbilityCharges(_hack, _hackCount);
        UpdateAbilityCharges(_disguise, _disgCount);
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
    private void UpdateReputationBar(Ability ability, Slider sliderToUpdate) => sliderToUpdate.value = ability.Reputation / 10;

    private void UpdateAbilityCharges(Ability ability, TextMeshProUGUI text) => text.text = ability.Charges.ToString();
}
