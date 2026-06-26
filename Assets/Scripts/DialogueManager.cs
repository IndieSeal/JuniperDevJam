using System;
using System.Collections;
using Febucci.UI;
using Febucci.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private GameObject dialoguePanel;
    [Space]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image characterIconImage;
    [SerializeField] private Sprite characterDefaultIcon;
    [SerializeField] private GameObject clickToContinue;
    private Action onDialogueOver;
    private Func<bool> continueCondition;
    private bool showClickToContinue;

    private TypewriterCore typewriter;

    protected override void Awake()
    {
        base.Awake();
        
        typewriter = dialogueText.GetComponent<TypewriterCore>();
        dialoguePanel.SetActive(false);
    }

    public void InitiateDialogue(string text, Func<bool> continueCondition, Sprite sprite = null, Action onDialogueOver = null, bool showClickToContinue = false)
    {
        typewriter.onTextShowed.RemoveListener(ManageDialogueFinished);
        StopAllCoroutines();
        
        dialoguePanel.SetActive(true);
        
        typewriter.onTextShowed.AddListener(ManageDialogueFinished);
        this.continueCondition = continueCondition;
        this.showClickToContinue = showClickToContinue;
        
        characterIconImage.sprite = sprite == null ? characterDefaultIcon : sprite;
        dialogueText.text = text;
        this.onDialogueOver = onDialogueOver;
    }

    private void ManageDialogueFinished()
    {
        typewriter.onTextShowed.RemoveListener(ManageDialogueFinished);
        StartCoroutine(ManageDialogue());
    }

    private IEnumerator ManageDialogue()
    {
        if(showClickToContinue) clickToContinue.SetActive(true);
        while (!continueCondition()) yield return null;

        clickToContinue.SetActive(false);
        dialogueText.text = "";
        dialoguePanel.SetActive(false);
        onDialogueOver?.Invoke();
    }
}