using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "GameData/Character", order = 1)]
public class CharacterInfo : ScriptableObject
{
    [SerializeField]
    public string CharName { get; private set; }

    [SerializeField]
    public Texture2D portrait { get; private set; }
}
