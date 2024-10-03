using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float originalFixedDeltaTime;
    public float originalTimeScale;
    private static TimeManager _instance;
    private bool isPaused = false; // Track if the game is paused

    public static TimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("TimeManager");
                _instance = obj.AddComponent<TimeManager>();
                DontDestroyOnLoad(obj); // Make sure it persists between scenes

                _instance.GetComponent<TimeManager>().originalFixedDeltaTime = Time.fixedDeltaTime;
                _instance.GetComponent<TimeManager>().originalTimeScale = Time.timeScale;
            }
            return _instance;
        }
    }

    // Method to trigger a slowdown, but it won't run if the game is paused
    public void TriggerSlowdown(float slowdownDuration, float slowdownFactor)
    {
        if (!isPaused) // Check if the game is paused before starting the slowdown
        {
            StartCoroutine(SlowdownFeedback(slowdownDuration, slowdownFactor));
        }
    }

    // Coroutine to manage the slowdown effect
    private IEnumerator SlowdownFeedback(float duration, float factor)
    {
        try
        {
            // Apply the time slowdown
            Time.timeScale = factor;
            Time.fixedDeltaTime = originalFixedDeltaTime * factor;

            // Wait for the specified duration in real time (ignoring Time.timeScale)
            yield return new WaitForSecondsRealtime(duration);
        }
        finally
        {
            // Reset the time scale and fixed delta time back to normal (1.0f)
            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = originalFixedDeltaTime;
        }
    }

    // Method to pause the TimeManager
    public void PauseGame()
    {
        isPaused = true;
        StopAllCoroutines();
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime;
    }

    // Method to resume the TimeManager
    public void ResumeGame()
    {
        isPaused = false;
    }
}
