using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class Constants
{
    public enum SceneIndexTable 
    {
        Init,
        Menu,
        Game,
        EndOfDay
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