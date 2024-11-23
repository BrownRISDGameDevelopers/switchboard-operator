using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SettingsMenu : MonoBehaviour
{
    public UnityEngine.UI.Button buttonBack;
    public UnityEngine.UI.Slider sfxSlider, musicSlider;
    private GameManager gameManager;

    void Start()
    {
        this.gameManager = FindObjectOfType<GameManager>();

        // Connect buttons to relevant functions
        buttonBack.onClick.AddListener(onBack);
        sfxSlider.onValueChanged.AddListener(changeSFXVolume);
        musicSlider.onValueChanged.AddListener(changeMusicVolume);
        
        changeSFXVolume(sfxSlider.value);
        changeMusicVolume(musicSlider.value);
    }

    void onBack()
    {
        Destroy(this.gameObject);
    }

    void changeMusicVolume(float value)
    {
        gameManager.SetMusicVolume(value);
    }
    void changeSFXVolume(float value)
    {
        gameManager.SetSFXVolume(value);
    }
}
