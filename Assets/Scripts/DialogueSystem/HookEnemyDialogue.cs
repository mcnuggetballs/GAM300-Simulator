using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookEnemyDialogue : MonoBehaviour
{
    [SerializeField]
    DialogueList dialogueList;
    [SerializeField]
    GameObject hookEnemyPrefab;
    [SerializeField]
    GameObject speakingEnemy;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DialogueSystem.Instance.StartDialogue(dialogueList, "0");
        }
    }
    public void DestroyMe()
    {
        Instantiate(hookEnemyPrefab, speakingEnemy.transform.position,speakingEnemy.transform.rotation);
        Destroy(speakingEnemy);
    }
}
