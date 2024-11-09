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
    TMPro.TextMeshProUGUI objective1GUI;
    [SerializeField]
    GameObject Objective1;
    [SerializeField]
    GameObject Objective2;
    [SerializeField]
    IngameLift lift;
    bool obj1Complete = false;
    bool obj2Complete = false;
    [SerializeField]
    GameObject checkMarkGameObject;
    [SerializeField]
    AudioSource audioSource;
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
        checkMarkGameObject.SetActive(false);
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
    private void Start()
    {
    }
    public void ShowObjective1()
    {
        StartCoroutine(ShowObjective());
    }
    IEnumerator PlayObjective1Audio()
    {
        yield return new WaitForSeconds(3.0f);
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
    private void Update()
    {
        if (!obj1Complete)
        {
            objective1GUI.text = "Kill all AI robots - " + (totalEnemies - currentEnemies).ToString() + "/" + totalEnemies;

            if (currentEnemies <= 0)
            {
                checkMarkGameObject.SetActive(true);
                AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.HackSounds[5], transform.position);
                StartCoroutine(ShowLines(1));
                obj1Complete = true;
                lift.enabled = true;
                StartCoroutine(PlayObjective1Audio());
                NotifSystem.Instance.SkipEnter(12);
            }
        }
        else if (!obj2Complete)
        {
            if (lift.GetOpenLiftCollider().GetCollisions() >= 1)
            {
                checkMarkGameObject.SetActive(true);
                AudioManager.instance.PlaySoundAtLocation(AudioManager.instance.HackSounds[5], transform.position);
                StartCoroutine(ShowLines(2));
                obj2Complete = true;
            }
        }
    }

    IEnumerator ShowLines(int lines)
    {
        animator.SetBool("ShowLine",true);
        if (lines == 1)
        {
            animator.SetTrigger("Complete1Line");
        }
        else
        {
            animator.SetTrigger("Complete2Line");
        }
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(CloseObjective());
        yield return new WaitForSeconds(1.0f);
        animator.SetBool("ShowLine", false);
        checkMarkGameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        if (obj1Complete)
        {
            Objective1.SetActive(false);
            Objective2.SetActive(true);
            StartCoroutine(ShowObjective());
        }    
    }
    IEnumerator CloseObjective()
    {
        animator.SetTrigger("HideObjective");
        yield return null;
    }
    IEnumerator ShowObjective()
    {
        animator.SetTrigger("ShowObjective");
        yield return null;
    }
}
