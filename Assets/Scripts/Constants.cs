using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class Constants
{
    public static Vector3 LINE_Z_OFFSET = new Vector3(0, 0, -0.25f);
    public static int LINE_SLICE_COUNT = 20;
    public static float GRAVITY = -0.4f;
    public static float KNOCKER_SHAKE = 0.3f;
    public static float JACK_SHAKE = 0.1f;
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