using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cutscene", menuName = "GameData/Cutscene", order = 1)]
public class Cutscene : ScriptableObject
{
    public Texture2D[] textures;
}
