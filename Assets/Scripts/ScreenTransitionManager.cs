using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;

public class ScreenTransitionManager : MonoBehaviour
{
    public enum TransitionMode
    {
        None,
        Fade,
        WipeLeft
    }
    public static ScreenTransitionManager Instance;

    public Animator transitionAnimator;
    public float transitionDuration = 1f;
    public string nextSceneName;
    protected TransitionMode transitionMode = TransitionMode.None;
    bool inTransition = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetTransitionMode(TransitionMode mode)
    {
        this.transitionMode = mode;
        if (transitionMode== TransitionMode.Fade)
        {
            transitionAnimator.SetBool("Fade", true);
            transitionAnimator.SetBool("WipeLeft", false);
        }
        else if (transitionMode== TransitionMode.WipeLeft)
        {
            transitionAnimator.SetBool("WipeLeft", true);
            transitionAnimator.SetBool("Fade", false);
        }
        else
        {
            transitionAnimator.SetBool("Fade", false);
            transitionAnimator.SetBool("WipeLeft", false);
        }
    }
    public void TransitionToScene(string sceneName)
    {
        if (inTransition)
            return;
        transitionAnimator.SetTrigger("StartTransition");
        nextSceneName = sceneName;
        inTransition = true;
    }

    private IEnumerator LoadScene()
    {
        SceneManager.LoadScene(nextSceneName);
        Time.timeScale = 1f;

        //wait for the scene to load
        yield return null;

        transitionAnimator.SetTrigger("EndTransition");
        inTransition = false;
    }
}
