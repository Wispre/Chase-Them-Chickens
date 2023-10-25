using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource Background;
    public AudioSource Sfx;

    public Slider BackgroundSlider;
    public Slider Sfx_Slider;

    public const string BACKGROUND = "Background";
    public const string SFX = "SFX";

    private void Awake()
    {
        SetSavedVolumeLevels();
        BackgroundSlider.value = Background.volume;
        Sfx_Slider.value = Sfx.volume;
    }

    public void SaveVolumeLevels()
    {
        PlayerPrefs.SetFloat(BACKGROUND, Background.volume);
        PlayerPrefs.SetFloat(SFX, Sfx.volume);
    }

    private void SetSavedVolumeLevels()
    {
        if (PlayerPrefs.HasKey(BACKGROUND) && PlayerPrefs.HasKey(SFX))
        {
            Background.volume = PlayerPrefs.GetFloat(BACKGROUND);
            Sfx.volume = PlayerPrefs.GetFloat(SFX);
        }
    }
}
