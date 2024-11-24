using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public UnityEngine.UI.Button buttonResume, buttonSettings, buttonQuit;
    public GameObject settingsMenuObject, emptyObject;
    private GameManager gameManager;
    private MusicManager musicManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        musicManager = gameManager.gameObject.GetComponentInChildren<MusicManager>();

        // Connect buttons to relevant functions
        buttonResume.onClick.AddListener(onResume);
        buttonQuit.onClick.AddListener(onQuit);
        buttonSettings.onClick.AddListener(onSettings);

        musicManager.immediateNPCSong();
        Time.timeScale = 0f;
    }

    void onSettings()
    {
        FindObjectOfType<VolumeManager>().GetComponent<AudioSource>().Play();
        Instantiate(settingsMenuObject);
    }

    // Start button pressed
    void onResume()
    {
        Time.timeScale = 1f;

        FindObjectOfType<VolumeManager>().GetComponent<AudioSource>().Play();
        Destroy(emptyObject);
        Destroy(gameObject);
    }

    // Quit button pressed
    void onQuit()
    {
        // Will not work in engine, but should close application on separate builds
        FindObjectOfType<VolumeManager>().GetComponent<AudioSource>().Play();
        Application.Quit();
    }
}