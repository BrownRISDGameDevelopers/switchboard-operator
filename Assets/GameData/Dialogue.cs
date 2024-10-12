using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


enum PortraitEmotion
{
    DEFAULT = 0,
}

[System.Serializable]
struct CharacterToTag
{
    Character character;
    string tag;
}

[CreateAssetMenu(fileName = "Character", menuName = "GameData/Character", order = 1)]
public class Dialogue : ScriptableObject
{
    [SerializeField]
    private Character character;
    [SerializeField]
    private PortraitEmotion portraitEmotion;
    [SerializeField]
    private string text;

    public string GetText()
    {
        return text;
    }


    [SerializeField]
    private string successTag;
    [SerializeField]
    private string failureTag;
    [SerializeField]
    private string ignoreTag;

    [SerializeField]
    private CharacterToTag[] characterMap;
}
