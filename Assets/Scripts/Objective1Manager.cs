using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective1Manager : MonoBehaviour
{
    // Singleton instance
    public static Objective1Manager Instance { get; private set; }

    Animator animator;
    public int totalEnemies;
    public int currentEnemies;
    [SerializeField]
    IngameLift lift;

    bool obj1Complete = false;
    bool obj2Complete = false;
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    List<GameObject> tasks;
    [SerializeField]
    GameObject taskPrefab;
    [SerializeField]
    Transform pivot;
    float distance = 150.0f;

    public void PlayToDoEnterSound()
    {
        AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.HackSounds[6], FindAnyObjectByType<PlayerHack>().transform.position);
    }
    public void PlayStrikeSound()
    {
        AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.HackSounds[Random.Range(7,9)], FindAnyObjectByType<PlayerHack>().transform.position);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        totalEnemies = 0;
        animator = GetComponent<Animator>();
        lift.enabled = false;
    }
    private void UpdateTaskPositions()
    {
        tasks.RemoveAll(item => item == null);

        int i = 0;
        foreach (var task in tasks)
        {
            Vector3 pos = Vector3.zero;
            pos.y = -i * distance;
            task.transform.localPosition = pos;
            ++i;
        }
    }
    private void UpdateTasks()
    {
        foreach (var task in tasks)
        {
            if (task.GetComponent<ObjectiveTask>().id == "KillAll")
            {
                task.GetComponent<ObjectiveTask>().textUI.text = "Kill all AI robots - " + (totalEnemies - currentEnemies).ToString() + "/" + totalEnemies;
                if (currentEnemies <= 0)
                {
                    task.GetComponent<ObjectiveTask>().CompleteTask();
                }
            }
        }
    }
    public void CreateObjectiveKillAllEnemies()
    {
        GameObject task = Instantiate(taskPrefab, pivot.position, Quaternion.identity, pivot);
        ObjectiveTask objTask = task.GetComponent<ObjectiveTask>();
        if (objTask != null)
        {
            objTask.CreateObjective("KillAll","Kill all AI robots - " + (totalEnemies - currentEnemies).ToString() + "/" + totalEnemies);
        }
        tasks.Add(task);
    }
    IEnumerator PlayObjectiveAudio()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
        yield return null;
    }
    
    private void Update()
    {
        UpdateTaskPositions();
        UpdateTasks();
        //if (!obj1Complete)
        //{
        //    objectiveText.text = "Kill all AI robots - " + (totalEnemies - currentEnemies).ToString() + "/" + totalEnemies;

        //    if (currentEnemies <= 0)
        //    {
        //        checkMarkGameObject.SetActive(true);
        //        AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.HackSounds[5], transform.position);
        //        obj1Complete = true;
        //        lift.enabled = true;
        //        StartCoroutine(PlayObjective1Audio());
        //        NotifSystem.Instance.SkipEnter(12);
        //    }
        //}
        //else if (!obj2Complete)
        //{
        //    if (lift.GetOpenLiftCollider().GetCollisions() >= 1)
        //    {
        //        checkMarkGameObject.SetActive(true);
        //        AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.HackSounds[5], transform.position);
        //        obj2Complete = true;
        //    }
        //}
    }
}
