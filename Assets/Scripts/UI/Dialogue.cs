using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    private GameObject _player;
    
    [SerializeField] private GameObject _dialogueBox;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private string _positiveComment, _neutralComment, _negativeComment;
    [SerializeField] private float _textSpeed = 0.05f;
    [SerializeField] private string comment;
    private string comment_intitial;
    [SerializeField] private int repType;
    
    private LockPickingAbility _lockpick;
    private KnockoutAbility _knock;
    private HackAbility _hack;
    private DisguiseAbility _disguise;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayerTransform().root.gameObject;
        _lockpick = _player.GetComponent<LockPickingAbility>();
        _knock =  _player.GetComponent<KnockoutAbility>();
        _hack = _player.GetComponent<HackAbility>();
        _disguise = _player.GetComponent<DisguiseAbility>();
        comment_intitial = comment;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (repType)
        {
            case 1:
                comment += _lockpick.AbilityLevel switch
                {
                    AbilityLevel.Positive => _positiveComment,
                    AbilityLevel.Neutral => _neutralComment,
                    AbilityLevel.Negative => _negativeComment,
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;
            case 2:
                comment += _knock.AbilityLevel switch
                {
                    AbilityLevel.Positive => _positiveComment,
                    AbilityLevel.Neutral => _neutralComment,
                    AbilityLevel.Negative => _negativeComment,
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;
            case 3:
                comment += _hack.AbilityLevel switch
                {
                    AbilityLevel.Positive => _positiveComment,
                    AbilityLevel.Neutral => _neutralComment,
                    AbilityLevel.Negative => _negativeComment,
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;
            case 4:
                comment += _disguise.AbilityLevel switch
                {
                    AbilityLevel.Positive => _positiveComment,
                    AbilityLevel.Neutral => _neutralComment,
                    AbilityLevel.Negative => _negativeComment,
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;
        }
        _dialogueBox.SetActive(true);
        StartDialogue();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _dialogueBox.SetActive(false);
        StopAllCoroutines();
        _dialogueText.text = "";
        comment = comment_intitial;
    }
    void StartDialogue() => StartCoroutine(TypeLine());

    IEnumerator TypeLine()
    {
        foreach (char c in comment.ToCharArray())
        {
            _dialogueText.text += c;
            yield return new WaitForSeconds(_textSpeed);
        }
    }
    
    
}
