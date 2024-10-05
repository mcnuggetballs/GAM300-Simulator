using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftButtonsSFXController : MonoBehaviour
{
    public AudioSource buttonSFX;  // General button press SFX

    // Function to play the general button SFX (this can be linked to any button's OnClick event)
    public void PlayButtonSFX()
    {
        buttonSFX.Play();
    }



}
