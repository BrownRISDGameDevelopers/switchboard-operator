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
    [SerializeField]
    private Day[] days;

    private DayManager dayManager;
    private LocationManager locationManager;
    private HashSet<Tag> tags;
    private int callsMissed = 0;
    private int callsRouted = 0;
    private int callsMessedUp = 0;
    private int money = 0;

    private static GameManager _existingGameManager = null;
    private static int currentDay = 0;

    void Awake()
    {
        if (_existingGameManager != null)
        {
            Debug.LogError("Critical Error in GameManager, multiple game managers");
            Destroy(this);
            return;
        }
        _existingGameManager = this;

        if (!TryGetComponent<LocationManager>(out locationManager))
        {
            Debug.LogError("Critical Error in GameManager, no location manager found in GameObject");
            return;
        }
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        LoadNewDay(days[currentDay]);
    }

    public void LoadNewDay(Day day)
    {
        dayManager = FindFirstObjectByType<DayManager>();
        if (dayManager != null)
        {
            dayManager.SetTagsReference(tags);
            dayManager.SetLocationReference(locationManager);
        }
    }

    public void EndCurrentDay()
    {
        // TODO: Potential for cutscenes and/or checks for early endings 
        //

        currentDay++;
        if (currentDay >= days.Length)
        {
            // End the game
            ReturnToMenu();
            return;
        }

        // Logic for cutscenes + early endings

        SceneManager.LoadScene((int) Constants.SceneIndexTable.EndOfDay);
    }

    public void EnterEndOfDay()
    {
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene((int) Constants.SceneIndexTable.Menu);
    }

    public void PayPlayer(int amount)
    {
        money += amount;
    }
}
