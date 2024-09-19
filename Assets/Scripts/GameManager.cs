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
}
