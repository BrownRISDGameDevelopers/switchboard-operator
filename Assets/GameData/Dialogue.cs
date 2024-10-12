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
    Character character;
    string tag;
}

[CreateAssetMenu(fileName = "Character", menuName = "GameData/Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
    [SerializeField]
    public Character Character { get; private set; }

    [SerializeField]
    public PortraitEmotion PortraitEmotion { get; private set; }

    [SerializeField]
    public string Text { get; private set; }



    [SerializeField]
    private string successTag;
    [SerializeField]
    private string failureTag;
    [SerializeField]
    private string ignoreTag;

    [SerializeField]
    private CharacterToTag[] characterMap;
}
