using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "GameData/Character", order = 1)]
public class Character : ScriptableObject
{
    public string charName;
    public Texture2D portrait;
}