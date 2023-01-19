using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.Profiling.LowLevel;

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

    [Space]
    [SerializeField] private GameObject _lockpickPrompt;
    [SerializeField] private GameObject _knockoutPrompt;
    [SerializeField] private GameObject _hackPrompt;
    [SerializeField] private GameObject _disguisePrompt;

    [Space]
    [SerializeField] private TextMeshProUGUI _lockpickComment;
    [SerializeField] private TextMeshProUGUI _knockoutComment;
    [SerializeField] private TextMeshProUGUI _hackComment;
    [SerializeField] private TextMeshProUGUI _disguiseComment;

    [Space]
    [SerializeField]private float _textSpeed = 0.025f; 


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

        _lockpick.repValueChange.AddListener((value) => ReputationChangePrompt(_lockpick, _lockpickPrompt, _lockpickComment, value));
        _knock.repValueChange.AddListener((value) => ReputationChangePrompt(_knock, _knockoutPrompt, _knockoutComment, value));
        _hack.repValueChange.AddListener((value) => ReputationChangePrompt(_hack, _hackPrompt, _hackComment, value));
        _disguise.repValueChange.AddListener((value) => ReputationChangePrompt(_disguise, _disguisePrompt, _disguiseComment, value));
        
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

    private void ReputationChangePrompt(Ability ability, GameObject prompt ,TextMeshProUGUI comment, float change)
    {
        if (change < 0)
        {
            prompt.SetActive(true);
            StartDialogue(comment, "- Not like that");
            //comment.text = "- Not like that";
            StartCoroutine(ExecuteAfterTime(2, prompt, comment));
        

        }
        else if (change > 0)
        {
            prompt.SetActive(true);
            StartDialogue(comment, "+ Nice, that's the way");
            //comment.text = "+ Nice, that's the way";
            StartCoroutine(ExecuteAfterTime(2, prompt, comment));
        }
        else
        {
            return;
        }
    }
    void StartDialogue(TextMeshProUGUI comment, string dialogue)
    {
        StartCoroutine(TypeLine(comment, dialogue));
    }

    IEnumerator TypeLine(TextMeshProUGUI comment, string dialogue)
    {
        foreach (char c in dialogue.ToCharArray())
        {
            comment.text += c;
            yield return new WaitForSeconds(_textSpeed);
        }
        
    }
    IEnumerator ExecuteAfterTime(float time, GameObject prompt, TextMeshProUGUI comment)
    {
        yield return new WaitForSeconds(time);
        comment.text = "";
        prompt.SetActive(false);

        // Code to execute after the delay
    }
}
