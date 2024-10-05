using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIbuttonsfx : MonoBehaviour
{
    [SerializeField] private AudioSource hoverSFX;    // Sound effect for hovering
    [SerializeField] private AudioSource clickSFX;    // Sound effect for clicking


    public void PlayHoverSFX()
    {
        if (hoverSFX != null)
        {
            hoverSFX.Play();
        }
    }

    public void PlayClickSFX()
    {
        if (clickSFX != null)
        {
            clickSFX.Play();
        }
    }
}
