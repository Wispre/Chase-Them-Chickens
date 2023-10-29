using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;

    public Slider Master_Slider;
    public Slider BackgroundSlider;
    public Slider Sfx_Slider;
    public Slider Ambience_Slider;

    public const string MIXER_MASTER = "MasterVolume";
    public const string MIXER_BACKGROUND_MUSIC = "BackgroudVolume";
    public const string MIXER_SFX = "SfxVolume";
    public const string MIXER_AMBIENCE = "AdjustedAmbienceVolume";

    private void Awake()
    {
        //LoadVolume();

        Master_Slider.value = PlayerPrefs.GetFloat(MIXER_MASTER, 1f);
        BackgroundSlider.value = PlayerPrefs.GetFloat(MIXER_BACKGROUND_MUSIC, 1f);
        Sfx_Slider.value = PlayerPrefs.GetFloat (MIXER_SFX, 1f);
        Ambience_Slider.value = PlayerPrefs.GetFloat(MIXER_AMBIENCE, 1f);
    }

    void Start()
    {
        LoadVolume();
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(MIXER_MASTER, Master_Slider.value);
        PlayerPrefs.SetFloat(MIXER_BACKGROUND_MUSIC, BackgroundSlider.value);
        PlayerPrefs.SetFloat(MIXER_SFX, Sfx_Slider.value);
        PlayerPrefs.SetFloat(MIXER_AMBIENCE, Ambience_Slider.value);
    }

    public void SetMasterVolume(float value)
    {
        mixer.SetFloat(MIXER_MASTER, Mathf.Log10(value) * 20f);
    }

    public void SetBackgroundMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_BACKGROUND_MUSIC, Mathf.Log10(value) * 20f);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20f);
    }

    public void SetAmbienceVolume(float value)
    {
        mixer.SetFloat(MIXER_AMBIENCE, Mathf.Log10(value) * 20f);
    }

    private void LoadVolume()
    {
        float masterVolume = PlayerPrefs.GetFloat(MIXER_MASTER, 1f);
        float backgroundMusicVolume = PlayerPrefs.GetFloat(MIXER_BACKGROUND_MUSIC, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(MIXER_SFX, 1f); ;
        float ambienceVolume = PlayerPrefs.GetFloat(MIXER_AMBIENCE, 1f);

        mixer.SetFloat(MIXER_MASTER, Mathf.Log10(masterVolume) * 20f);
        mixer.SetFloat(MIXER_BACKGROUND_MUSIC, Mathf.Log10(backgroundMusicVolume) * 20f);
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(sfxVolume) * 20f);
        mixer.SetFloat(MIXER_AMBIENCE, Mathf.Log10(ambienceVolume) * 20f);
    }
}
