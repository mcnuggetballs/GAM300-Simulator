using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;
using JetBrains.Annotations;
using UnityEngine.Events;

[Serializable]
public class DialogueEvent : UnityEvent { };
[Serializable]
public class Options
{
    public string option;
    public DialogueEvent optionAction;
}

[Serializable]
public class Dialogue
{
    public enum Side
    {
        Left,
        Right
    }
    public string name;
    public string text;
    public Sprite PlayerIcon;
    public DialogueEvent OnGoNext;
    public Side side;
}

[Serializable]
public class ButtonAndText
{
    public Button button;
    public int optionIndex;
    public TextMeshProUGUI text;
}


public class DialogueSystem : MonoBehaviour
{
    static DialogueSystem instance;
    public static DialogueSystem Instance
    {
        set
        {
            instance = value;
        }
        get
        {
            return instance;
        }
    }

    public TextMeshProUGUI nameTextBox;
    public TextMeshProUGUI dialogueTextBox;
    public Image player1Image;
    public Image player2Image;
    public float textSpeed;
    protected List<Dialogue> dialogues;
    public GameObject dialogueUI;

    public List<ButtonAndText> buttonsList;
    public GameObject optionsUI;

    public delegate void OnDialogueCompleteDelegate();
    public event OnDialogueCompleteDelegate OnDialogueComplete;

    private int index;
    private int optionIndex;
    public void StartDialogue(DialogueList dialogueList, string dialogueID)
    {
        List<Dialogue> dialogue = dialogueList.GetDialoguesFromID(dialogueID);
        if (dialogue.Count > 0)
        {
            dialogueUI.SetActive(true);
            index = 0;
            dialogueTextBox.text = string.Empty;
            dialogues = dialogue;
            StartCoroutine(TypeLine());
        }
        else
        {
            if (OnDialogueComplete != null)
            {
                OnDialogueComplete();
                Debug.LogWarning("DialogueCompleted");
            }
        }
    }

    IEnumerator TypeLine()
    {
        UpdateNameAndImages();
        foreach (char c in dialogues[index].text.ToCharArray())
        {
            dialogueTextBox.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        dialogueTextBox.text = string.Empty;
        nameTextBox.text = string.Empty;
        player1Image.sprite = null;
        player1Image.color = Color.clear;
        player2Image.sprite = null;
        player2Image.color = Color.clear;
        dialogueUI.SetActive(false);
    }

    // Update is called once per frame
    public void ClickNext(bool ignoreOptions)
    {
        if (dialogueTextBox.text == dialogues[index].text)
        {
                NextLine();
        }
        else
        {
            StopAllCoroutines();
        }
    }

    void UpdateNameAndImages()
    {
        nameTextBox.text = dialogues[index].name;
        if (player1Image.sprite == null)
        {
            player1Image.color = Color.clear;
        }
        else
        {
            player1Image.color = Color.white;
        }
        if (player2Image.sprite == null)
        {
            player2Image.color = Color.clear;
        }
        else
        {
            player2Image.color = Color.white;
        }
    }

    void NextLine()
    {
        if (index < dialogues.Count - 1)
        {
            ++index;
            dialogueTextBox.text = string.Empty;
            nameTextBox.text = string.Empty;
            player1Image.sprite = null;
            player1Image.color = Color.clear;
            player2Image.sprite = null;
            player2Image.color = Color.clear;
            StartCoroutine(TypeLine());
            //UpdateNameAndImages();
        }
        else
        {
            dialogueUI.SetActive(false);
            dialogues[index].OnGoNext.Invoke();
            
            if (OnDialogueComplete != null)
            {
                OnDialogueComplete();
                Debug.LogWarning("DialogueCompleted");
            }
        }
    }
}
