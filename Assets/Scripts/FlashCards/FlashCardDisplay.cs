using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashCardDisplay : MonoBehaviour
{
    public static FlashCardDisplay Instance { get; private set; }
    [SerializeField]
    GameObject display;
    [SerializeField]
    Image cardImage;
    public List<Sprite> displayImages;
    [SerializeField]
    float switchTime = 1.0f;
    float timer = 0.0f;
    int index = 0;
    bool activated = false;
    Animator animator;
    float clickAwayTime = 1.5f;
    float clickAwayTimer = 0.0f;
    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
    }
    public void Activate(List<Sprite> images)
    {
        clickAwayTimer = 0.0f;
        display.SetActive(true);
        displayImages = images;
        activated = true;
        TimeManager.Instance.PauseGame();
        animator.SetBool("Show", true);
    }
    private void Update()
    {
        clickAwayTimer += Time.unscaledDeltaTime;
        if (activated)
        {
            if (!TimeManager.Instance.IsGamePaused())
            {
                TimeManager.Instance.PauseGame();
            }
            cardImage.sprite = displayImages[index];
            timer += Time.unscaledDeltaTime;
            if (timer >= switchTime)
            {
                timer = 0.0f;
                ++index;
                if (index >= displayImages.Count)
                {
                    index = 0;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (activated)
        {
            if (!TimeManager.Instance.IsGamePaused())
            {
                TimeManager.Instance.PauseGame();
            }
        }
    }
    public void Deactivate()
    {
        if (clickAwayTimer >= clickAwayTime)
        {
            animator.SetBool("Show", false);
        }
    }

    public void Resume()
    {
        display.SetActive(false);
        activated = false;
        TimeManager.Instance.ResumeGame();
    }
}
