using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ending", menuName = "GameData/Ending", order = 1)]
public class Ending : ScriptableObject
{
    public Tag[] requiredTags;
    public string Text;
}
