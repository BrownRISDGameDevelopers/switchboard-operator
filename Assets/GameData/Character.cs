using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "GameData/Character", order = 1)]
public class Character : ScriptableObject
{
    [SerializeField]
    private string charName;

    [SerializeField]
    private Texture2D portrait;
}
