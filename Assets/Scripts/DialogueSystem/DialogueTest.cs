using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    [SerializeField]
    DialogueList list;
    // Start is called before the first frame update
    void Start()
    {
        DialogueSystem.Instance.StartDialogue(list, "Hello");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
