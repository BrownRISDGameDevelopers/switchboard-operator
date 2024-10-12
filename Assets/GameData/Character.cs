using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "GameData/Character", order = 1)]
public class CharacterInfo : ScriptableObject
{
    [SerializeField]
    private string charName;

    [SerializeField]
    private Texture2D portrait;
}
