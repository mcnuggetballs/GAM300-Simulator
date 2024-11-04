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

    public TextMeshProUGUI currentDialogueTextBox;
    public float textSpeed;
    protected List<Dialogue> dialogues;
    public GameObject dialogueUI;
    List<GameObject> bubblesSpawned = new List<GameObject>();

    public delegate void OnDialogueCompleteDelegate();
    public event OnDialogueCompleteDelegate OnDialogueComplete;

    private int index;
    private int optionIndex;

    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public void BumpBubbles(float bump)
    {
        for(int i = 0; i < bubblesSpawned.Count; i++)
        {
            Vector3 bubblePos = bubblesSpawned[i].transform.position;
            bubblePos.y += bump;
            bubblesSpawned[i].transform.position = bubblePos;
        }
    }
    public void StartDialogue(DialogueList dialogueList, string dialogueID)
    {
        List<Dialogue> dialogue = dialogueList.GetDialoguesFromID(dialogueID);
        if (dialogue.Count > 0)
        {
            dialogueUI.SetActive(true);
            index = 0;
            if (dialogue[index] != null)
            {
                if (dialogue[index].side == Dialogue.Side.Left)
                {
                    GameObject pref = Instantiate(leftPrefab, dialogueUI.transform);
                    DialogueBubble bubble = pref.GetComponent<DialogueBubble>();
                    if (bubble)
                    {
                        bubblesSpawned.Add(pref);
                        currentDialogueTextBox = bubble.textBox;
                        currentDialogueTextBox.text = "";
                        bubble.nameText.text = dialogue[index].name;
                    }
                }
                else
                {
                    GameObject pref = Instantiate(rightPrefab, dialogueUI.transform);
                    DialogueBubble bubble = pref.GetComponent<DialogueBubble>();
                    if (bubble)
                    {
                        bubblesSpawned.Add(pref);
                        currentDialogueTextBox = bubble.textBox;
                        currentDialogueTextBox.text = "";
                        bubble.nameText.text = dialogue[index].name;
                    }
                }
            }
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
        foreach (char c in dialogues[index].text.ToCharArray())
        {
            currentDialogueTextBox.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        dialogueUI.SetActive(false);
    }

    // Update is called once per frame
    public void ClickNext(bool ignoreOptions)
    {
        Debug.LogError("Clicked");
        if (currentDialogueTextBox == null)
            return;
        if (currentDialogueTextBox.text == dialogues[index].text)
        {
            Debug.LogError("Next Line");
            NextLine();
        }
        else
        {
            Debug.LogError("Nope");
            Debug.LogError(currentDialogueTextBox.text + " " + dialogues[index].text);
            StopAllCoroutines();
        }
    }

    void NextLine()
    {
        if (index < dialogues.Count - 1)
        {
            ++index;
            if (dialogues[index] != null)
            {
                BumpBubbles(140);
                if (dialogues[index].side == Dialogue.Side.Left)
                {
                    GameObject pref = Instantiate(leftPrefab, dialogueUI.transform);
                    DialogueBubble bubble = pref.GetComponent<DialogueBubble>();
                    if (bubble)
                    {
                        bubblesSpawned.Add(pref);
                        currentDialogueTextBox = bubble.textBox;
                        currentDialogueTextBox.text = "";
                        bubble.nameText.text = dialogues[index].name;
                    }
                }
                else
                {
                    GameObject pref = Instantiate(rightPrefab, dialogueUI.transform);
                    DialogueBubble bubble = pref.GetComponent<DialogueBubble>();
                    if (bubble)
                    {
                        bubblesSpawned.Add(pref);
                        currentDialogueTextBox = bubble.textBox;
                        currentDialogueTextBox.text = "";
                        bubble.nameText.text = dialogues[index].name;
                    }
                }
            }
            StartCoroutine(TypeLine());
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
