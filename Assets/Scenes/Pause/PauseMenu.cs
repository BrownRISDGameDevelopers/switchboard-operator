using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public UnityEngine.UI.Button buttonResume, buttonSettings, buttonQuit;
    public GameObject gameManagerObject;
    
    // Start is called before the first frame update
    void Start()
    {
        // Connect buttons to relevant functions
        buttonResume.onClick.AddListener(onResume);
        buttonSettings.onClick.AddListener(onSettings);
        buttonQuit.onClick.AddListener(onQuit);
    }

    // Resume button pressed
    void onResume()
    {
        Instantiate(gameManagerObject);
    }

    // Settings button pressed
    void onSettings()
    {
        // Will not work in engine, but should close application on separate builds
        Application.Quit();
    }

    // Quit button pressed
    void onQuit()
    {
        // Will not work in engine, but should close application on separate builds
        Application.Quit();
    }
}