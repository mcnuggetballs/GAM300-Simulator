using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    protected bool hackMode;
    protected bool promptsDisabled = false;
    [Header("XP")]
    protected int experiencePoints = 0;
    public int GetXP()
    {
        return experiencePoints;
    }
    public void AddExperience(int value)
    {
        experiencePoints += value;
    }
    public void MinusExperience(int value)
    {
        experiencePoints -= value;
        if (experiencePoints < 0)
        {
            experiencePoints= 0;
        }
    }
    //singleton
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }
    public bool GetPromptsDisabled()
    {
        return promptsDisabled;
    }
    public void TogglePrompts()
    {
        promptsDisabled = !promptsDisabled;
    }
    public bool GetHackMode()
    {
        return hackMode;
    }

    public void ToggleHackMode(bool value)
    {
        hackMode = value;
    }

    public void TriggerSlowdown(float slowdownDuration, float slowdownFactor)
    {
        TimeManager.Instance.TriggerSlowdown(slowdownDuration, slowdownFactor);
    }
    public void TriggerCameraShake(CameraShake.ShakeSettings settings, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        CameraShakeManager.Instance.StartShake(settings, camera);
    }
}
