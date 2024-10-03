using Cinemachine.PostFX;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

public class CameraHack : MonoBehaviour
{
    [SerializeField]
    Animator cameraAnimator;
    [SerializeField]
    CinemachineVolumeSettings cinemachineVolumeSettings;

    private ColorAdjustments colorAdjustments;
    private UnityEngine.Rendering.Universal.Vignette vignette;
    private FilmGrain filmGrain;

    [SerializeField]
    float colourGradingTemperature;
    [SerializeField]
    float colourGradingSaturation;
    [SerializeField]
    float vignetteIntensity;
    [SerializeField]
    float grainIntensity;
    [SerializeField]
    float grainResponse;
    [SerializeField]
    Animator hackModeOverlayAnimator;
    // Update is called once per frame
    void Update()
    {
        cinemachineVolumeSettings.m_Profile.TryGet(out colorAdjustments);
        cinemachineVolumeSettings.m_Profile.TryGet(out vignette);
        cinemachineVolumeSettings.m_Profile.TryGet(out filmGrain);
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.SetValue(new ClampedFloatParameter(colourGradingSaturation,-100,100));
        }
        if (vignette != null)
        {
            vignette.intensity.SetValue(new ClampedFloatParameter(vignetteIntensity, 0, 1));
        }
        if (filmGrain != null)
        {
            filmGrain.intensity.SetValue(new ClampedFloatParameter(grainIntensity, 0, 1));
            filmGrain.response.SetValue(new ClampedFloatParameter(grainIntensity, 0, 1));
        }
        cameraAnimator.SetBool("Hack", GameManager.Instance.GetHackMode());
        if (hackModeOverlayAnimator)
            hackModeOverlayAnimator.SetBool("HackMode", GameManager.Instance.GetHackMode());
    }
}
