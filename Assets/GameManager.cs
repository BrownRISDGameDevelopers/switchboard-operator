using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine;

public static class GameManager
{
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
}