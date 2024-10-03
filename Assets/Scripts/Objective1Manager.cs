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
    bool obj1Complete = false;
    bool obj2Complete = false;
    [SerializeField]
    GameObject checkMarkGameObject;

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
        EnemyControllerRB[] enemies = FindObjectsOfType<EnemyControllerRB>();
        foreach (EnemyControllerRB enemy in enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                ++totalEnemies;
            }
        }
        currentEnemies = totalEnemies;
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        StartCoroutine(ShowObjective());
    }
    private void Update()
    {
        if (!obj1Complete)
        {
            objective1GUI.text = "Kill all AI robots - " + (totalEnemies - currentEnemies).ToString() + "/" + totalEnemies;

            if (currentEnemies <= 0)
            {
                checkMarkGameObject.SetActive(true);
                StartCoroutine(ShowLines(1));
                obj1Complete = true;
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
