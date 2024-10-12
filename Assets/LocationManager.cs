using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Location
{
    public bool Valid;
    public int Index;

    public char Letter;
    public char Number;
}


[System.Serializable]
public struct CharacterInfoLocStruct
{
    public CharacterInfo info;
    public Location loc;
}

public class LocationManager : MonoBehaviour
{
    [SerializeField]
    CharacterInfoLocStruct[] initInfo;

    Dictionary<CharacterInfo, Location> positionToCharacter = new Dictionary<CharacterInfo, Location>();


    void Awake()
    {
        foreach (CharacterInfoLocStruct infoStruct in initInfo)
        {
            positionToCharacter.Add(infoStruct.info, infoStruct.loc);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void RandomizeLocations() { }


    public List<CharacterInfo> GetCharacterList()
    {
        return new List<CharacterInfo>(positionToCharacter.Keys);
    }

    public List<Location> GetLocationList()
    {
        return new List<Location>(positionToCharacter.Values);
    }

    public Location GetLocationFromCharacter(CharacterInfo character)
    {
        if (positionToCharacter.TryGetValue(character, out Location loc))
        {
            return loc;
        }
        return new Location { Valid = false, Index = 0, Letter = '0', Number = '0' };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
