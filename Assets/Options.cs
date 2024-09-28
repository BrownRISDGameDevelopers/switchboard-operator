using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Options
{
    private static Constants.Difficulty difficulty = Constants.Difficulty.Medium;

    public static Constants.Difficulty getDifficulty()
    {
        return difficulty;
    }

    public static void setDifficulty(Constants.Difficulty value)
    {
        difficulty = value;
    }
}
