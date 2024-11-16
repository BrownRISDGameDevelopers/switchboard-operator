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
public struct CharacterToTags
{
    public CharacterInfo character;
    public Tag[] tags;
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

    public Tag[] requiredTags;
    public Tag[] disallowedTags;
    public DialogueLine[] Lines;

    [Header("Tags")]
    public Tag[] successTags;
    public Tag[] failureTags;
    public Tag[] ignoreTags;

    public CharacterToTags[] characterMap;


    // nullable
    public Tag[] GetTagsFromCharacter(CharacterInfo info)
    {
        foreach (CharacterToTags characterTags in characterMap)
        {
            if (characterTags.character == info)
            {
                return characterTags.tags;
            }
        }
        return null;
    }
}
