using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct Location
{
    public bool Valid;

    //[HideInInspector]
    //public int Index;
    // A - Z
    public char Letter;
    // 0 - 9
    public int Number;

    public int GetIndex(int columns)
    {
        int row = (int)(Letter - 65);
        int col = Number - 1;
        return row * columns + col;
    }

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
    public CharacterInfo[] goodCharacterList;
    
    [SerializeField]
    CharacterInfoLocStruct[] initInfo;

    private Dictionary<Location, CharacterInfo> positionToCharacter = new Dictionary<Location, CharacterInfo>();

    void Awake()
    {
        // foreach (CharacterInfoLocStruct infoStruct in initInfo)
        // {
        //     positionToCharacter.Add(infoStruct.loc, infoStruct.info);
        // }

        // RandomizeLocations();
        generateLocations();
    }

    private void generateLocations()
    {
        Location[] goodLocationsList = new Location[goodCharacterList.Length];
        for (int i = 0; i < goodCharacterList.Length; i++)
        {
            print("CHARACTERS ADDED: " + goodCharacterList[i].name);
            goodLocationsList[i] = new Location { Valid = false, Letter = (char)(math.floor(i / 5) + 65), Number = i % 5 + 1 };
        }

        goodLocationsList = goodLocationsList.OrderBy( x => UnityEngine.Random.value ).ToArray();

        for (int i = 0; i < goodCharacterList.Length; i++){
            positionToCharacter.Add(goodLocationsList[i], goodCharacterList[i]);
            print(goodLocationsList[i]);
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

        // Locations should be completely randomized if random
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
            print("CHARACTERS PRESENT: " + pair.Value.name);
            if (pair.Value == location)
            {
                return pair.Key;
            }
        }
        return new Location { Valid = false, Letter = '0', Number = '0' };
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
