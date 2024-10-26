using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public enum PortraitEmotion
{
    DEFAULT = 0,
}

[System.Serializable]
struct CharacterToTag
{
    CharacterInfo character;
    string tag;
}

[CreateAssetMenu(fileName = "Character", menuName = "GameData/Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
    public CharacterInfo Character;
    public PortraitEmotion PortraitEmotion;
    public string Text;

    public Tag successTag;
    public Tag failureTag;
    public Tag ignoreTag;

    private CharacterToTag[] characterMap;
}
