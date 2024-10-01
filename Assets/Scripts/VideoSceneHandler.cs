using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoSceneHandler : MonoBehaviour
{
    [SerializeField]
    VideoPlayer videoPlayer;
    bool finished = false;
    private void Update()
    {
        if (videoPlayer != null && finished == false)
        {
            if (!videoPlayer.isPlaying && videoPlayer.frame > 0)
            {
                finished = true;
                SceneTransition.Instance.SetTransitionMode(ScreenTransitionManager.TransitionMode.Fade);
                SceneTransition.Instance.TransitionToScene("Whiteboxed");
            }
        }
    }
}
