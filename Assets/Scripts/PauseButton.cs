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
    public GameObject pauseMenuObject, emptyObject;
    void Start()
    {
        // Connect buttons to relevant functions
        buttonPause.onClick.AddListener(onClick);
    }

    void onClick()
    {
        FindObjectOfType<VolumeManager>().GetComponent<AudioSource>().Play();

        GameObject eo = Instantiate(emptyObject);
        GameObject pmo = Instantiate(pauseMenuObject);

        pmo.GetComponent<PauseMenu>().emptyObject = eo;
    }
}
