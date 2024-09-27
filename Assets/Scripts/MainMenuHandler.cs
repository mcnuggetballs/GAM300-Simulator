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
        animator.SetTrigger("Start");
    }
    public void SetScene(string sceneName)
    {
        SceneTransition.Instance.SetTransitionMode(ScreenTransitionManager.TransitionMode.WipeLeft);
        SceneTransition.Instance.TransitionToScene(sceneName);
    }
}
