using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Make sure some timing thing for the ordered calls?
 */

[System.Serializable]
public struct DialogueHolder
{
    [SerializeField]
    public Tag[] requiredTags;
    public Dialogue dialogue;
}

[System.Serializable]
public struct SingleDayDialogueList
{
    public DialogueHolder[] dialogue;
}

// Day thing
// Each day has array of potential ordered call pools
// Within those, you specify individual dialogues which have required tags
// Then, upon something being selected, the first one with the required tags is picked
[CreateAssetMenu(fileName = "Day", menuName = "GameData/Day", order = 1)]
public class Day : ScriptableObject
{
    public SingleDayDialogueList[] RandomizedCallPool;
    public SingleDayDialogueList[] OrderedCallPool;
}
