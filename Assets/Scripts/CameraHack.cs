using Cinemachine.PostFX;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class CameraHack : MonoBehaviour
{
    [SerializeField]
    Animator cameraAnimator;
    [SerializeField]
    CinemachinePostProcessing cinemachinePostProcessing;

    private Grain grain;
    private ColorGrading colorGrading;
    private Vignette vignette;

    [SerializeField]
    float colourGradingTemperature;
    [SerializeField]
    float colourGradingSaturation;
    [SerializeField]
    float vignetteIntensity;
    [SerializeField]
    float grainIntensity;
    // Update is called once per frame
    void Update()
    {
        cinemachinePostProcessing.m_Profile.TryGetSettings(out grain);
        cinemachinePostProcessing.m_Profile.TryGetSettings(out colorGrading);
        cinemachinePostProcessing.m_Profile.TryGetSettings(out vignette);

        // Modify grain settings
        if (grain != null)
        {
            grain.intensity.value = grainIntensity;
        }
        if (colorGrading != null)
        {
            colorGrading.temperature.value = colourGradingTemperature;
        }
        if (vignette != null)
        {
            vignette.intensity.value = vignetteIntensity;
        }
        cameraAnimator.SetBool("Hack", GameManager.Instance.GetHackMode());
    }
}
