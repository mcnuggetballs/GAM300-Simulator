using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DialogueSequence
{
    public string dialogueID;
    public List<Dialogue> dialogues;
}
public class DialogueList : MonoBehaviour
{
    public List<DialogueSequence> list;

    public List<Dialogue> GetDialoguesFromID(string dialogueID)
    {
        foreach(var dialogueList in list)
        {
            if (dialogueList.dialogueID == dialogueID)
            {
                return dialogueList.dialogues;
            }
        }
        Debug.LogError("Dialogue Not Found");
        return null;
    }

    public List<DialogueSequence> GetDialogueSequenceList()
    {
        return list;
    }
}
