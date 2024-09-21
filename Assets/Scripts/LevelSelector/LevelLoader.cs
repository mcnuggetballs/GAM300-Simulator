using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [Header("Transition's Variables")]
    public Animator ScreenTransition;
    public float ScreenTransitionTime = 1.0f;
    //public bool ToggleTransition; //true means on

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LiftTrigger.hasEntered)
        {
/*            if (ToggleTransition)
            {*/
                StartCoroutine(LoadScene("LevelSelector"));
                LiftTrigger.hasEntered = false;
/*            }
            else
            {
                SceneManager.LoadScene("LevelSelector");
                LiftTrigger.hasEntered = false;
            }*/
        }
    }

    IEnumerator LoadScene(string sceneName)
    {
        ScreenTransition.SetTrigger("Start");

        yield return new WaitForSeconds(ScreenTransitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
