using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public UnityEngine.UI.Button buttonStart, buttonQuit;
    public GameObject gameManger;

    void Start()
    {
        // Connect buttons to relevant functions
        buttonStart.onClick.AddListener(onStart);
        buttonQuit.onClick.AddListener(onQuit);
    }

    // Start button pressed
    void onStart()
    {
        SceneManager.LoadScene("Game");
        Instantiate(gameManger);
    }

    // Quit button pressed
    void onQuit()
    {
        // Will not work in engine, but should close application on separate builds
        Application.Quit();
    }
}
