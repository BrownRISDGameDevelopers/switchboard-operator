using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine;

// Overarching game manager
// not globally accessible, but only one should exist at a time 
// handles day changes, moving between scenes, stores tags, etc. 
public class GameManager : MonoBehaviour
{
    private DayManager dayManager;
    private HashSet<Tag> tags;

    private LocationManager locationManager;

    [SerializeField]
    private Day[] days;
    private int currentDay = 0;

    private int callsMissed = 0;
    private int callsRouted = 0;
    private int callsMessedUp = 0;
    private int money = 0;


    private static GameManager _checkManager = null;

    void Awake()
    {
        if (_checkManager != null)
        {
            Debug.LogError("Critical Error in GameManager, multiple game managers");
            Destroy(this);
            return;
        }
        _checkManager = this;

        if (!TryGetComponent<LocationManager>(out locationManager))
        {
            Debug.LogError("Critical Error in GameManager, no location manager found in gameobject");
            return;
        }
        DontDestroyOnLoad(this);
    }


    void Destroy()
    {
        if (_checkManager != this)
            return;
        _checkManager = null;
    }

    public void LoadNewDay(Day day)
    {

        enterGameScene();

        dayManager = FindFirstObjectByType<DayManager>();
        if (dayManager != null)
        {
            dayManager.SetTagsReference(tags);
            dayManager.SetLocationReference(locationManager);
        }
    }

    public void OnGameManagerEndGame()
    {
        // TODO: Potential for cutscenes and/or checks for early endings 
        //

        currentDay++;
        if (currentDay >= days.Length)
        {
            // End the game
            returnMainMenu();
            return;
        }

        // Logic for cutscenes + early endings



        LoadNewDay(days[currentDay]);
    }



    public void enterGameScene()
    {
        SceneManager.LoadScene((int)Constants.SceneIndexTable.Game);
    }

    public void enterEndOfDayScene()
    {
        SceneManager.LoadScene((int)Constants.SceneIndexTable.EndOfDay);
    }

    public void returnMainMenu()
    {
        SceneManager.LoadScene((int)Constants.SceneIndexTable.Menu);
    }

    public void payPlayer(int amount)
    {
        money += amount;
    }
}
