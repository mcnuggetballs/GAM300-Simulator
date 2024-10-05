using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
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

    public AudioMixer audioMixer;
    protected string bgmVolumeParameter = "Master";
    float fadeDuration = 2.0f;

    private void Awake()
    {
        audioMixer = Resources.Load<AudioMixer>("AudioMixer");
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
        StartCoroutine(FadeOutBGM());
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
        StartCoroutine(FadeInBGM());
    }

    float originalBGMVolume = 0;
    private IEnumerator FadeOutBGM()
    {
        audioMixer.GetFloat(bgmVolumeParameter, out originalBGMVolume);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            Debug.LogError(Mathf.Lerp(originalBGMVolume, -80f, t / fadeDuration));
            float newVolume = Mathf.Lerp(originalBGMVolume, -80f, t / fadeDuration);
            audioMixer.SetFloat(bgmVolumeParameter, newVolume);
            yield return null;
        }
    }

    private IEnumerator FadeInBGM()
    {
        float originalVolume = originalBGMVolume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float newVolume = Mathf.Lerp(-80f, originalVolume, t / fadeDuration);
            audioMixer.SetFloat(bgmVolumeParameter, newVolume);
            yield return null;
        }

        audioMixer.SetFloat(bgmVolumeParameter, originalVolume);
    }
}
