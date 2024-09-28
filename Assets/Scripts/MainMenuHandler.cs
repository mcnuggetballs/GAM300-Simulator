using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void PressStartButton()
    {
        animator.SetTrigger("Close");
        animator.SetBool("Start",true);
    }
    public void PressHowToPlayButton()
    {
        animator.SetTrigger("Close");
        animator.SetBool("HowToPlay", true);
    }
    public void CloseHowToPlayButton()
    {
        animator.SetTrigger("Close");
        animator.SetBool("HowToPlay", false);
    }
    public void SetScene(string sceneName)
    {
        SceneTransition.Instance.SetTransitionMode(ScreenTransitionManager.TransitionMode.WipeLeft);
        SceneTransition.Instance.TransitionToScene(sceneName);
    }
}
