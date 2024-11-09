using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EndingType
{
    EndOfDays = 0,
    InstantTagsAcquired = 1,
    EndOfAnyDay = 2,
}

[CreateAssetMenu(fileName = "Ending", menuName = "GameData/Ending", order = 1)]
public class Ending : ScriptableObject
{
    public Tag[] requiredTags;
    public string Text;
    public Texture2D[] portrait;
    public bool instantEndOnRequirements = false;
}
