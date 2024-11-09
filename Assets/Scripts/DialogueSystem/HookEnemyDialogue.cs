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
    [SerializeField]
    List<AudioClip> audioClips;
    [SerializeField]
    public EntityDeathEvent deathEvent;
    public float deathEventDelay = 0.0f;
    [SerializeField]
    public GameObject idBadgePrefab;
    public EntityDeathEvent idBadgePickupEvent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DialogueSystem.Instance.StartDialogue(dialogueList, "0");
        }
    }
    public void DestroyMe()
    {
        GameObject spawnedEnemy = Instantiate(hookEnemyPrefab, speakingEnemy.transform.position,speakingEnemy.transform.rotation);
        Entity theEntity = spawnedEnemy.GetComponent<Entity>();
        if (theEntity != null)
        {
            theEntity.SetDeathDelayDuration(deathEventDelay);
            theEntity.deathEvent = deathEvent;
            theEntity.deathDrop = idBadgePrefab;
            theEntity.deathDropPickupEvent = idBadgePickupEvent;
        }
        Destroy(speakingEnemy);
    }

    public void PlayClip(int id)
    {
        DialogueSystem.Instance.SetAudioClip(audioClips[id]);
        DialogueSystem.Instance.PlayAudioClip();
    }
}
