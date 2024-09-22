using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [Header("Level Buttons")]
    [SerializeField]
    private List<GameObject> level1;
    [SerializeField]
    private List<GameObject> level2;
    [SerializeField]
    private List<GameObject> level3;
    [SerializeField]
    private GameObject ButtonPenthouse;

    [Header("Level Counter")]
    //[SerializeField]
    public static int currentLevel = 1;

    void Awake()
    {
        //SwitchLevel(1);
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    //reload scene for a new level
    public static void SwitchLevel(int level)
    {
        switch (level)
        {
            case 1:
                currentLevel = 1;
                SceneManager.LoadScene(0);
                break;
            case 2:
                currentLevel = 2;
                SceneManager.LoadScene(0);
                break;
            case 3:
                currentLevel = 3;
                SceneManager.LoadScene(0);
                break;
        }
    }
}
