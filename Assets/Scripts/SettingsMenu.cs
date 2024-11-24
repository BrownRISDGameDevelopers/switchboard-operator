using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public UnityEngine.UI.Button buttonBack;
    public UnityEngine.UI.Slider sfxSlider, musicSlider;
    private VolumeManager volumeManager;

    void Start()
    {
        volumeManager = FindObjectOfType<VolumeManager>();

        // Connect buttons to relevant functions
        buttonBack.onClick.AddListener(onBack);
        sfxSlider.onValueChanged.AddListener(changeSFXVolume);
        musicSlider.onValueChanged.AddListener(changeMusicVolume);

        changeMusicVolume(volumeManager.currentMusicSliderValue);
        changeSFXVolume(volumeManager.currentSFXSliderValue);
    }

    void onBack()
    {
        FindObjectOfType<VolumeManager>().GetComponent<AudioSource>().Play();
        Destroy(gameObject);
    }

    void changeMusicVolume(float value)
    {
        musicSlider.value = value;
        volumeManager.SetMusicVolume(value);
    }
    void changeSFXVolume(float value)
    {
        sfxSlider.value = value;
        volumeManager.SetSFXVolume(value);
    }
}
