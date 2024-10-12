using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine;

public static class GameManager
{
    private static uint callsMissed = 0;
    private static uint callsRouted = 0;
    private static uint callsMessedUp = 0;
    private static uint daysElapsed = 0;
    private static int money = 0;
    public static void enterGameScene()
    {
        SceneManager.LoadScene((int) Constants.SceneIndexTable.Game);
    }

    public static void enterEndOfDayScene()
    {
        SceneManager.LoadScene((int) Constants.SceneIndexTable.EndOfDay);
    }
    public static void returnMainMenu()
    {
        SceneManager.LoadScene((int) Constants.SceneIndexTable.Menu);
    }

    public static void payPlayer(int amount)
    {
        money += amount;
    }
}