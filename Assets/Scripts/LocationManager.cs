using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Location
{
    public bool Valid;
    public int Index;
    // A - Z
    public char Letter;
    // 0 - 9
    public int Number;

    public override bool Equals(object obj)
    {
        if (!(obj is Location))
            return false;
        Location cmp = (Location)obj;
        return (cmp.Letter == Letter && cmp.Number == Number);
    }
    public override int GetHashCode()
    {
        return ((int)Letter << (int)Number);
    }


    public override string ToString() => $"Location: [{Letter}, {Number}]";
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

    private Dictionary<Location, CharacterInfo> positionToCharacter = new Dictionary<Location, CharacterInfo>();

    void Awake()
    {
        foreach (CharacterInfoLocStruct infoStruct in initInfo)
        {
            positionToCharacter.Add(infoStruct.loc, infoStruct.info);
        }
    }

    public List<Location> GenerateLocations(Switchboard switchboard)
    {
        Switch[,] switches = switchboard.GetSwitches();
        List<Location> locList = new List<Location>();

        foreach (Switch s in switches)
        {
            Location locData = s.locationData;
            locList.Add(locData);
        };

        return locList;
    }
    private void RandomizeLocations()
    {
        System.Random rand = new System.Random();

        Dictionary<Location, CharacterInfo> newPositionsToCharacter = new Dictionary<Location, CharacterInfo>();
        List<CharacterInfo> charList = GetCharacterList();
        List<Location> locList = GetLocationList();

        foreach (CharacterInfo c in charList)
        {
            int index = rand.Next(0, locList.Count);
            Location newLoc = locList[index];
            locList.Remove(newLoc);

            newPositionsToCharacter.Add(newLoc, c);
        }

        positionToCharacter = newPositionsToCharacter;
    }
    private void RandomizeLocations(List<Location> locList)
    {
        System.Random rand = new System.Random();

        Dictionary<Location, CharacterInfo> newPositionsToCharacter = new Dictionary<Location, CharacterInfo>();
        List<CharacterInfo> charList = GetCharacterList();

        foreach (CharacterInfo c in charList)
        {
            int index = rand.Next(0, locList.Count);
            Location newLoc = locList[index];
            locList.Remove(newLoc);

            newPositionsToCharacter.Add(newLoc, c);
        }

        positionToCharacter = newPositionsToCharacter;
    }
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
        return null;
    }

}
