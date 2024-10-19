using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Location
{
    public bool Valid;
    public int Index;

    public char Letter;
    public int Number;
}


[System.Serializable]
public struct CharacterInfoLocStruct
{
    public Location loc;
    public CharacterInfo info;
}

public class LocationManager : MonoBehaviour
{
    [SerializeField]
    CharacterInfoLocStruct[] initInfo;

    Dictionary<Location, CharacterInfo> positionToCharacter = new Dictionary<Location, CharacterInfo>();


    void Awake()
    {
        foreach (CharacterInfoLocStruct infoStruct in initInfo)
        {
            positionToCharacter.Add(infoStruct.loc, infoStruct.info);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void RandomizeLocations() { }


    public List<CharacterInfo> GetCharacterList()
    {
        return new List<CharacterInfo>(positionToCharacter.Values);
    }

    public List<Location> GetLocationList()
    {
        return new List<Location>(positionToCharacter.Keys);
    }

    public Location GetLocationFromCharacter(CharacterInfo location)
    {
        foreach (KeyValuePair<Location, CharacterInfo> pair in positionToCharacter)
        {
            if (pair.Value == location)
            {
                return pair.Key;
            }
        }
        return new Location { Valid = false, Index = 0, Letter = '0', Number = '0' };
    }
    // Nullable
    public CharacterInfo GetCharacterFromLocation(Location location)
    {
        if (positionToCharacter.TryGetValue(location, out CharacterInfo character))
        {
            return character;
        }
        return null;//new Location { Valid = false, Index = 0, Letter = '0', Number = '0' };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
