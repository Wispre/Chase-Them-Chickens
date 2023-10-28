using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;

    [Header("Sliders")]
    public Slider BackgroundSlider;
    public Slider Sfx_Slider;
    public const string MIXER_BACKGROUND_MUSIC = "BackgroudVolume";
    public const string MIXER_SFX = "SfxVolume";

    private void Awake()
    {
        BackgroundSlider.value = PlayerPrefs.GetFloat(MIXER_BACKGROUND_MUSIC, 1f);
        Sfx_Slider.value = PlayerPrefs.GetFloat(MIXER_SFX, 1f);
    }

    private void Start()
    {
        LoadVolumeSettings();
    }

    private void OnDisable()
    {
        SaveVolumeSettings();
    }
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat(MIXER_BACKGROUND_MUSIC, BackgroundSlider.value);
        PlayerPrefs.SetFloat(MIXER_SFX, Sfx_Slider.value);
    }

    public void SetBackgroundMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_BACKGROUND_MUSIC, Mathf.Log10(value) * 20f);
    }

    public void SetSfxVolume(float value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20f);
    }

    private void LoadVolumeSettings()
    {
        SetBackgroundMusicVolume(PlayerPrefs.GetFloat(MIXER_BACKGROUND_MUSIC, 1f));
        SetSfxVolume(PlayerPrefs.GetFloat(MIXER_SFX, 1f));
    }
}
