using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    [SerializeField]
    public AudioMixer sfxMixer;

    [SerializeField]
    public AudioMixer musicMixer;

    public float currentMusicSliderValue = 0.5f;
    public float currentSFXSliderValue = 0.5f;

    public void SetMusicVolume(float value)
    {
        currentMusicSliderValue = value;
        musicMixer.SetFloat("Volume", Mathf.Log10(value) * 20);
    }
    public void SetSFXVolume(float value)
    {
        currentSFXSliderValue = value;
        sfxMixer.SetFloat("Volume", Mathf.Log10(value) * 20);
    }

    void Start()
    {
        return;
    }
}