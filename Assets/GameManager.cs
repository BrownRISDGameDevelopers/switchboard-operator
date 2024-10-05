using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DayManager dayManager;
    private HashSet<string> tags;

    void Awake() {
        dayManager = FindFirstObjectByType<DayManager>(); 
        
        if (dayManager != null) { 
            dayManager.SetTagsReference(tags);
        }
        
        DontDestroyOnLoad(this);
    }

    public void enterGameScene()
    {
        SceneManager.LoadScene((int) Constants.SceneIndexTable.Game);
    }

    public void enterEndOfDayScene()
    {
        SceneManager.LoadScene((int) Constants.SceneIndexTable.EndOfDay);
    }
    public void returnMainMenu()
    {
        SceneManager.LoadScene((int) Constants.SceneIndexTable.Menu);
    }
}
