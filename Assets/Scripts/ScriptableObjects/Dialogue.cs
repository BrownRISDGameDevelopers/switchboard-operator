using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public enum PortraitEmotion
{
    DEFAULT = 0,
    EXTRA_1 = 1,
    EXTRA_2 = 2,
}

[System.Serializable]
struct CharacterToTag
{
    CharacterInfo character;
    string tag;
}

[System.Serializable]
public class DialogueLine
{
    public string text = "";
    public PortraitEmotion portraitType = PortraitEmotion.DEFAULT;
    public float speedScale = 1.0f;
}

[CreateAssetMenu(fileName = "Character", menuName = "GameData/Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
    public CharacterInfo FromCharacter;
    public CharacterInfo ToCharacter;
    public DialogueLine[] Lines;

    public Tag successTag;
    public Tag failureTag;
    public Tag ignoreTag;

    private CharacterToTag[] characterMap;
}
