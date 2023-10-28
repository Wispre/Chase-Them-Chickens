using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;

    public Slider BackgroundSlider;
    public Slider Sfx_Slider;

    public const string MIXER_BACKGROUND_MUSIC = "BackgroudVolume";
    public const string MIXER_SFX = "SfxVolume";

    private void Awake()
    {
        LoadVolume();

        BackgroundSlider.value = PlayerPrefs.GetFloat(MIXER_BACKGROUND_MUSIC, 1f);
        Sfx_Slider.value = PlayerPrefs.GetFloat (MIXER_SFX, 1f);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(MIXER_BACKGROUND_MUSIC, BackgroundSlider.value);
        PlayerPrefs.SetFloat(MIXER_SFX, Sfx_Slider.value);
    }

    public void SetBackgroundMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_BACKGROUND_MUSIC, Mathf.Log10(value) * 20f);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20f);
    }

    private void LoadVolume()
    {
        float backgroundMusicVolume = PlayerPrefs.GetFloat(MIXER_BACKGROUND_MUSIC, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(MIXER_SFX, 1f); ;

        mixer.SetFloat(MIXER_BACKGROUND_MUSIC, Mathf.Log10(backgroundMusicVolume) * 20f);
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(sfxVolume) * 20f);
    }
}
