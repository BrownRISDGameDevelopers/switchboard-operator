using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "GameData/Character", order = 1)]
public class CharacterInfo : ScriptableObject
{
    public string CharName;

    public Sprite Portrait;
    public Sprite ExtraPortrait1;
    public Sprite ExtraPortrait2;
}
