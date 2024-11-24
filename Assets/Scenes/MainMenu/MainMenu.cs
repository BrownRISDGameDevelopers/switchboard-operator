using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public UnityEngine.UI.Button buttonStart, buttonQuit, buttonHowToPlay, buttonSettings, buttonCredits;
    public GameObject tutorialObject, gameManagerObject, settingsMenuObject, creditsObject;

    void Start()
    {
        // Connect buttons to relevant functions
        buttonStart.onClick.AddListener(onStart);
        buttonQuit.onClick.AddListener(onQuit);
        buttonSettings.onClick.AddListener(onSettings);
        buttonCredits.onClick.AddListener(onCredits);
        buttonHowToPlay.onClick.AddListener(onTutorial);
    }

    void onCredits()
    {
        Instantiate(creditsObject);
    }

    void onSettings()
    {
        Instantiate(settingsMenuObject);
    }

    void onTutorial()
    {
        Instantiate(tutorialObject);
    }

    // Start button pressed
    void onStart()
    {
        gameManagerObject = Instantiate(gameManagerObject);
    }

    // Quit button pressed
    void onQuit()
    {
        // Will not work in engine, but should close application on separate builds
        Application.Quit();
    }
}
