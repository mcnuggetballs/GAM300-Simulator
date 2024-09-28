using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager1 : MonoBehaviour
{
    bool isPaused = false;
    bool isDead = false;
    [SerializeField]
    GameObject pauseUI;
    [SerializeField]
    GameObject deadUI;
    public Entity playerEntity;
    private void Awake()
    {
        PlayerHack playerHack = FindAnyObjectByType<PlayerHack>();
        if (playerHack)
            playerEntity = playerHack.GetComponent<Entity>();
    }
    public void RetryScene()
    {
        ChangeScene(SceneManager.GetActiveScene().name);
    }
    public void ChangeScene(string sceneName)
    {
        SceneTransition.Instance.SetTransitionMode(ScreenTransitionManager.TransitionMode.WipeLeft);
        SceneTransition.Instance.TransitionToScene(sceneName);
    }
    public void SetDead()
    {
        isDead = true;
        StartCoroutine(ShowDeathScreen());
    }

    private IEnumerator ShowDeathScreen()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        deadUI.SetActive(true);
        Time.timeScale = 0f;
        yield return null;
    }

    public void PressPause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PressPause();
        }
        if (playerEntity)
        {
            if (playerEntity.GetCurrentHealth() <= 0 && isDead == false)
            {
                SetDead();
            }
        }

    }

    void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
