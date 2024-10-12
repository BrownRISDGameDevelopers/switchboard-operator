using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DayManager dayManager;
    private HashSet<string> tags;

    private LocationManager locationManager;

    [SerializeField]
    private Day[] days;
    private static uint callsMissed = 0;
    private static uint callsRouted = 0;
    private static uint callsMessedUp = 0;
    private static uint daysElapsed = 0;
    private static int money = 0;

    void Awake()
    {

        if (!TryGetComponent<LocationManager>(out locationManager))
        {
            Debug.LogError("Critical Error in GameManager, no location manager found in gameobject");
            return;
        }


        dayManager = FindFirstObjectByType<DayManager>();
        if (dayManager != null)
        {
            dayManager.SetTagsReference(tags);
        }

        DontDestroyOnLoad(this);
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
