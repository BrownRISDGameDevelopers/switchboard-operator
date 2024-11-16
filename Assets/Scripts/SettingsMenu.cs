using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SettinsMenu : MonoBehaviour
{
    public UnityEngine.UI.Button buttonBack;

    void Start()
    {
        // Connect buttons to relevant functions
        buttonBack.onClick.AddListener(onBack);
    }

    void onBack()
    {
        Destroy(this.gameObject);
    }
}
