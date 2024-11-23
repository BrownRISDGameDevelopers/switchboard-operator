using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PauseButton : MonoBehaviour
{
    public UnityEngine.UI.Button buttonPause;
    public GameObject pauseMenuObject;
    void Start()
    {
        // Connect buttons to relevant functions
        buttonPause.onClick.AddListener(onClick);
    }

    void onClick()
    {
        Instantiate(pauseMenuObject);
    }
}
