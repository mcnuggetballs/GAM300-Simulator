using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    protected bool hackMode;
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
