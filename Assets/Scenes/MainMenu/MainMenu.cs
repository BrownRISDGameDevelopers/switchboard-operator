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
        FindObjectOfType<VolumeManager>().GetComponent<AudioSource>().Play();
        Instantiate(creditsObject);
    }

    void onSettings()
    {
        FindObjectOfType<VolumeManager>().GetComponent<AudioSource>().Play();
        Instantiate(settingsMenuObject);
    }

    void onTutorial()
    {
        FindObjectOfType<VolumeManager>().GetComponent<AudioSource>().Play();
        Instantiate(tutorialObject);
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(1.0f);
        gameManagerObject = Instantiate(gameManagerObject);
    }

    // Start button pressed
    void onStart()
    {
        FindObjectOfType<VolumeManager>().GetComponent<AudioSource>().Play();
        transform.GetComponent<AudioSource>().Play();

        StartCoroutine(Spawn());
    }

    // Quit button pressed
    void onQuit()
    {
        // Will not work in engine, but should close application on separate builds
        FindObjectOfType<VolumeManager>().GetComponent<AudioSource>().Play();
        Application.Quit();
    }
}
