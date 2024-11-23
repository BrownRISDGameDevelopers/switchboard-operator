using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class Constants
{
    public static Vector3 LINE_Z_OFFSET = new Vector3(0, 0, -0.25f);
    public static int LINE_SLICE_COUNT = 20;
    public static float GRAVITY = -0.4f;
    public static float KNOCKER_SHAKE = 0.4f;
    public static float JACK_OFF_SHAKE = 0.4f;
    public static float JACK_IN_SHAKE = 0.3f;
    public enum SceneIndexTable
    {
        Init = 0,
        Menu = 1,
        Game = 2,
        Ending = 3,
        EndOfDay,
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
