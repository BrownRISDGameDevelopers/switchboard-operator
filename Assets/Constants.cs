using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class Constants
{
    public enum SceneIndexTable 
    {
        Init = 0,
        Menu = 1
    }
    public enum GameStates
    {
        Initial,
        StartMenu,
        InGame,
        EndOfDay
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    
}