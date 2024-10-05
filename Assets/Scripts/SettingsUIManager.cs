using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] TMPro.TextMeshProUGUI masterVolumeText;
    [SerializeField] Slider bgmVolumeSlider;
    [SerializeField] TMPro.TextMeshProUGUI bgmVolumeText;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] TMPro.TextMeshProUGUI sfxVolumeText;

    private void Start()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        float masterVolume;
        mixer.GetFloat("Master", out masterVolume);
        masterVolumeSlider.value = Mathf.Pow(10, masterVolume / 20);
        UpdateVolumeText(masterVolumeText, masterVolumeSlider.value);

        float bgmVolume;
        mixer.GetFloat("BGM", out bgmVolume);
        bgmVolumeSlider.value = Mathf.Pow(10, bgmVolume / 20);
        UpdateVolumeText(bgmVolumeText, bgmVolumeSlider.value);

        float sfxVolume;
        mixer.GetFloat("SFX", out sfxVolume);
        sfxVolumeSlider.value = Mathf.Pow(10, sfxVolume / 20);
        UpdateVolumeText(sfxVolumeText, sfxVolumeSlider.value);
    }

    public void SetMasterVolume(float sliderValue)
    {
        if (sliderValue == 0)
        {
            mixer.SetFloat("Master", -80f);
            UpdateVolumeText(masterVolumeText, 0);
        }
        else
        {
            float volume = Mathf.Log10(sliderValue) * 20;
            mixer.SetFloat("Master", volume);
            UpdateVolumeText(masterVolumeText, sliderValue);
        }
    }

    public void SetBGMVolume(float sliderValue)
    {
        if (sliderValue == 0)
        {
            mixer.SetFloat("BGM", -80f);
            UpdateVolumeText(bgmVolumeText, 0);
        }
        else
        {
            float volume = Mathf.Log10(sliderValue) * 20;
            mixer.SetFloat("BGM", volume);
            UpdateVolumeText(bgmVolumeText, sliderValue);
        }
    }

    public void SetSFXVolume(float sliderValue)
    {
        if (sliderValue == 0)
        {
            mixer.SetFloat("SFX", -80f);
            UpdateVolumeText(sfxVolumeText, 0);
        }
        else
        {
            float volume = Mathf.Log10(sliderValue) * 20;
            mixer.SetFloat("SFX", volume);
            UpdateVolumeText(sfxVolumeText, sliderValue);
        }
    }

    private void UpdateVolumeText(TMPro.TextMeshProUGUI textElement, float sliderValue)
    {
        int percentage = Mathf.RoundToInt(sliderValue * 100);
        textElement.text = percentage.ToString();
    }
}
