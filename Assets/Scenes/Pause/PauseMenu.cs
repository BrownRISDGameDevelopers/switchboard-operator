using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public UnityEngine.UI.Button buttonResume, buttonSettings, buttonQuit;
    public GameObject settingsMenuObject;
    private GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        this.gameManager = FindObjectOfType<GameManager>();

        // Connect buttons to relevant functions
        buttonResume.onClick.AddListener(onResume);
        buttonQuit.onClick.AddListener(onQuit);
        buttonSettings.onClick.AddListener(onSettings);

        Time.timeScale = 0f;
    }

    void onSettings()
    {
        Instantiate(settingsMenuObject);
    }

    // Start button pressed
    void onResume()
    {
        Time.timeScale = 1f;
        Destroy(this.gameObject);
    }

    // Quit button pressed
    void onQuit()
    {
        // Will not work in engine, but should close application on separate builds
        Application.Quit();
    }
}