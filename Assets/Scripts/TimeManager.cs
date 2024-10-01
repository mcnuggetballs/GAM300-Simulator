using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float originalFixedDeltaTime;
    public float originalTimeScale;
    private static TimeManager _instance;

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
    public void TriggerSlowdown(float slowdownDuration, float slowdownFactor)
    {
        StartCoroutine(SlowdownFeedback(slowdownDuration, slowdownFactor));
    }

    private IEnumerator SlowdownFeedback(float duration, float factor)
    {

        try
        {
            // Apply the time slowdown
            Time.timeScale = factor;
            Time.fixedDeltaTime = originalFixedDeltaTime * factor;
            // Wait for the specified duration in real time
            yield return new WaitForSecondsRealtime(duration);
        }
        finally
        {
            // Reset the time scale back to normal (1.0f)
            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = originalFixedDeltaTime;
        }
    }
}
