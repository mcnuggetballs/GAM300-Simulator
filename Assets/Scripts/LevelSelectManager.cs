using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    public int level = -1;
    public int difficulty = -1;
    public ToggleButtonSprite selectedLevelButton;
    public ToggleButtonSprite selectedDifficultyButton;
    public static LevelSelectManager Instance { get; private set; }
    public AudioSource proceedSFX;

    private Animator animator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (animator)
        {
            if (selectedLevelButton)
            {
                animator.SetBool("LevelSelected", true);
                if (selectedDifficultyButton)
                {
                    animator.SetBool("DifficultySelected", true);
                }
                else
                {
                    animator.SetBool("DifficultySelected", false);
                }
            }
            else
            {
                animator.SetBool("LevelSelected", false);
                if (selectedDifficultyButton)
                {
                    animator.SetBool("DifficultySelected", false);
                    selectedDifficultyButton.OnButtonClick();
                }
            }
        }
    }

    public void Proceed()
    {
        if (animator)
        {
            if (selectedLevelButton && selectedDifficultyButton)
            {
                // Play the proceed SFX
                proceedSFX.Play();

                // Trigger the Proceed animation after SFX
                StartCoroutine(PlayProceedAnimation());

                // Disable interaction with level buttons
                Image[] levelButtons = FindObjectsOfType<Image>();
                foreach (Image levelButton in levelButtons)
                {
                    levelButton.raycastTarget = false;
                }
            }
        }
    }

    IEnumerator PlayProceedAnimation()
    {
        // Wait for the proceed SFX to finish before triggering the animation
        yield return new WaitForSeconds(proceedSFX.clip.length);

        // Trigger the Proceed animation
        animator.SetTrigger("Proceed");

        // Start the coroutine to load the next level
        StartCoroutine(ProceedToLevel());
    }

    public void InvalidButton()
    {
        if (animator)
        {
            animator.SetTrigger("InvalidButton");
        }
    }

    IEnumerator ProceedToLevel()
    {
        yield return new WaitForSeconds(3.0f);
        SceneTransition.Instance.SetTransitionMode(ScreenTransitionManager.TransitionMode.Fade);
        if (level == 1)
        {
            SceneTransition.Instance.TransitionToScene("Whiteboxed");
        }
        yield return null;
    }
}
