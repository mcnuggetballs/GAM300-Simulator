using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NotifSystem : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> notifs;
    [SerializeField]
    Image notifImage;
    int step = 0;
    bool active = false;
    Animator animator;
    [SerializeField]
    AudioSource audioSource;
    public void PlayPopupSound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
    private static NotifSystem _instance;

    public static NotifSystem Instance
    {
        get
        {
            return _instance;
        }
    }
    IEnumerator Enter(int value, float waitTime = 0.0f)
    {
        yield return new WaitForSeconds(waitTime);
        if (value == 0)
        {
            notifImage.sprite = notifs[value];
        }
        else if (value == 1)
        {
            notifImage.sprite = notifs[value];
        }
        else if (value == 2)
        {
            notifImage.sprite = notifs[value];
        }
        else if (value == 3)
        {
            notifImage.sprite = notifs[value];
        }
        else if (value == 4)
        {
            notifImage.sprite = notifs[value];
            StartCoroutine(CompleteAfterSeconds(5.0f, 4));
        }
        else if (value == 5)
        {
            notifImage.sprite= notifs[value];
            StartCoroutine(CompleteAfterSeconds(5.0f, 5));
        }
        else if (value == 6)
        {
            notifImage.sprite = notifs[value];
            StartCoroutine(CompleteAfterSeconds(5.0f, 6));
        }
        else if (value == 7)
        {
            notifImage.sprite = notifs[value];
            StartCoroutine(CompleteAfterSeconds(5.0f, 7));
        }
        else if (value == 8)
        {
            notifImage.sprite = notifs[value];
            StartCoroutine(CompleteAfterSeconds(5.0f, 8));
        }
        else if (value == 9)
        {
            notifImage.sprite = notifs[value];
            StartCoroutine(CompleteAfterSeconds(5.0f, 9));
        }
        else if (value == 10)
        {
            notifImage.sprite = notifs[value];
            StartCoroutine(CompleteAfterSeconds(5.0f, 10));
        }
        else if (value == 11)
        {
            Objective1Manager.Instance.CreateObjectiveKillAllEnemies();
            notifImage.sprite = notifs[value];
            StartCoroutine(CompleteAfterSeconds(5.0f, 11));
        }
        else if (value == 12)
        {
            notifImage.sprite = notifs[value];
            StartCoroutine(CompleteAfterSeconds(5.0f, 12));
        }
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Enter",true);
        active = true;
    }
    public void SkipEnter(int value)
    {
        step = value;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("enter"))
        {
            animator.SetBool("Enter", false);
        }
        else
        {
            animator.SetTrigger("Skip");
        }
        StartCoroutine(Enter(value,1.0f));
    }
    public void SkipEnterOne(int value)
    {
        if (step != value-1)
            return;
        ++step;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("enter"))
        {
            animator.SetBool("Enter", false);
        }
        else
        {
            animator.SetTrigger("Skip");
        }
        StartCoroutine(Enter(value, 1.0f));
    }
    public void CompleteStep(int value)
    {
        if (value == 0)
        {
            if (step == 0)
            {
                animator.SetBool("Enter", false);
                step = 1;
                active = false;
                StartCoroutine(Enter(1));
                StartCoroutine(CompleteAfterSeconds(5.0f, 1));
            }
        }
        else if (value == 1)
        {
            if (step == 1)
            {
                animator.SetBool("Enter", false);
                step = 2;
                active = false;
                StartCoroutine(Enter(2,1.0f));
                StartCoroutine(CompleteAfterSeconds(5.0f, 2));
            }
        }
        else if (value == 2)
        {
            if (step == 2)
            {
                animator.SetBool("Enter", false);
                step = 3;
                active = false;
                StartCoroutine(Enter(3, 1.0f));
                StartCoroutine(CompleteAfterSeconds(5.0f, 3));
            }
        }
        else if (value == 3)
        {
            if (step == 3)
            {
                animator.SetBool("Enter", false);
                step = 4;
                active = false;
            }
        }
        else if (value == 4)
        {
            if (step == 4)
            {
                animator.SetBool("Enter", false);
                step = 5;
                active = false;
            }
        }
        else if (value == 5)
        {
            if (step == 5)
            {
                animator.SetBool("Enter", false);
                step = 6;
                active = false;
            }
        }
        else if (value == 6)
        {
            if (step == 6)
            {
                animator.SetBool("Enter", false);
                step = 7;
                active = false;
            }
        }
        else if (value == 7)
        {
            if (step == 7)
            {
                animator.SetBool("Enter", false);
                step = 8;
                active = false;
            }
        }
        else if (value == 8)
        {
            if (step == 8)
            {
                animator.SetBool("Enter", false);
                step = 9;
                active = false;
            }
        }
        else if (value == 9)
        {
            if (step == 9)
            {
                animator.SetBool("Enter", false);
                step = 10;
                active = false;
            }
        }
        else if (value == 10)
        {
            if (step == 10)
            {
                animator.SetBool("Enter", false);
                step = 11;
                active = false;
                StartCoroutine(Enter(11, 1.0f));
                StartCoroutine(CompleteAfterSeconds(5.0f, 11));
            }
        }
        else if (value == 11)
        {
            if (step == 11)
            {
                animator.SetBool("Enter", false);
                step = 12;
                active = false;
            }
        }
        else if (value == 12)
        {
            if (step == 12)
            {
                animator.SetBool("Enter", false);
                step = 13;
                active = false;
            }
        }
    }
    IEnumerator CompleteAfterSeconds(float seconds,int step)
    {
        yield return new WaitForSeconds(seconds);
        CompleteStep(step);
    }
    private void Awake()
    {
        _instance = this;
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        StartCoroutine(Enter(0));
        StartCoroutine(CompleteAfterSeconds(5.0f,0));
    }
    private void Update()
    {
    }
}
