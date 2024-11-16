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
    public string text;
    public Sprite[] anim;
    public float secsPerFrame = 0.25f;
    //public bool instantEndOnRequirements = false;
    public EndingType endingType;
}
