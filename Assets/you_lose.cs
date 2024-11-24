using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class you_lose : MonoBehaviour
{
    public UnityEngine.UI.Button buttonPlayAgain, buttonHome;
    public GameObject tutorialObject, gameManagerObject, settingsMenuObject, creditsObject;


    void Start()
    {
        // Connect buttons to relevant functions
        buttonPlayAgain.onClick.AddListener(onPlayAgain);
        buttonHome.onClick.AddListener(onHome);
    }

    void onPlayAgain(){
        SceneManager.LoadScene(0);
    }

    void onHome(){
        SceneManager.LoadScene(0);
    }
}
