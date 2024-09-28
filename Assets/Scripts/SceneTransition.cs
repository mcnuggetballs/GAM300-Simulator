using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SceneTransition
{
    private static SceneTransition instance;

    public static ScreenTransitionManager screenTransitor;
    public static ScreenTransitionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SceneTransition();
                if (screenTransitor == null)
                {
                    screenTransitor = GameObject.Instantiate(Resources.Load("SceneTransitor", typeof(GameObject))).GetComponent<ScreenTransitionManager>();
                    GameObject.DontDestroyOnLoad(screenTransitor.gameObject);
                }
            }
            return screenTransitor;
        }
    }


}
