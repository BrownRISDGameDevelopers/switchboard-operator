using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * logic
 *  From day
 *  pick from random pool on timer
 *  thing is now selectable
 *  
 *  On jack pickup
 *      if location is currently in pool, add one set of tags
 *      if not, add to another
 *      if wait too long, add third
 *
 *  
 *
 */

class CallData
{
    public float curTimer;
    public CharacterInfo fromCharacter;
    public CharacterInfo toCharacter;

    public CallData(CharacterInfo fromChar, CharacterInfo toChar, float timer)
    {
        curTimer = timer;
        fromCharacter = fromChar;
        toCharacter = toChar;
    }
}

// Next steps
// Once caller is connected, need to store that data in dictionary
// then need function for when everthing is set
// then need that to handle different cases, and add and remove tags as necessary

class JackCallersHeldData
{
    public CharacterInfo from;
    public CharacterInfo to;
}


// TODO: we need way for calls to be ignored, and calls need to be on UI
public class DayManager : MonoBehaviour
{

    private HashSet<Tag> tagsReference;
    private Day currentDay;
    private LocationManager locationManager;
    private Switchboard _switchboard;

    public int strikesLeft = 3;

    // For incoming call logic
    private List<CallData> _callList = new List<CallData>();
    private float _callTimeoutTime = 1.0f;
    private float _callAddTimeMin = 1.0f;
    private float _callAddTimeMax = 10.0f;
    private float _callNextTimer = 5.0f;

    private HashSet<CharacterInfo> _callingCharacters = new HashSet<CharacterInfo>();
    private HashSet<Dialogue> _previousCalls = new HashSet<Dialogue>();
    private List<DialogueHolder> _randomizedCalls;


    // Jackin it 
    private Dictionary<int, Jack> _idToJack = new Dictionary<int, Jack>();
    private HashSet<int> _placedJacks = new HashSet<int>();
    private Dictionary<int, JackCallersHeldData> _jackSetToHeldCallers = new Dictionary<int, JackCallersHeldData>();

    void Start()
    {
        _randomizedCalls = GetOrderedAvailableDialogue(currentDay.RandomizedCallPool);
        // Call upon switch board
        // connect events
        Jack.onJackPlaced += OnJackPlaced;
        Jack.onJackTaken += OnJackRemoved;
        SetSwitchboard();




    }
    // Update is called once per frame
    void Update()
    {
        CheckForIncomingCalls();
        CheckCalls();

    }

    // nullable
    private Jack GetAssociatedJack(Jack jack)
    {
        return _idToJack.GetValueOrDefault(jack.jackID % 2 == 0 ? jack.jackID + 1 : jack.jackID - 1);
    }

    public void SetupDayManager(HashSet<Tag> newTags, LocationManager locManager, Day day)
    {
        currentDay = day;
        tagsReference = newTags;
        locationManager = locManager;
    }

    public void SetTagsReference(HashSet<Tag> newTags)
    {
        tagsReference = newTags;
    }

    public void SetLocationReference(LocationManager locManager)
    {
        locationManager = locManager;
    }

    private void SetSwitchboard()
    {
        _switchboard = FindAnyObjectByType<Switchboard>();
        if (!_switchboard)
        {
            return;
        }
        Jack[] jacks = _switchboard.GetJacks();
        foreach (Jack jack in jacks)
        {
            _idToJack.Add(jack.jackID, jack);
        }
    }


    private void CheckForIncomingCalls()
    {
        _callNextTimer -= Time.deltaTime;
        if (_callNextTimer > 0)
        {
            return;
        }
        Dialogue newCall = PopRandomizedCall();

        // TODO DISPlAY DIALOGUE

        // ADD RELEVANT INFO
        _callList.Add(new CallData(newCall.FromCharacter, newCall.ToCharacter, 10.0f));
        AddDialoguetoData(newCall);

        _callNextTimer = Random.Range(_callAddTimeMin, _callAddTimeMax);
    }

    private void AddDialoguetoData(Dialogue toAdd)
    {
        _callingCharacters.Add(toAdd.FromCharacter);
        _callingCharacters.Add(toAdd.ToCharacter);
    }

    private void RemoveDialogueFromData(Dialogue toRemove)
    {
        _callingCharacters.Remove(toRemove.FromCharacter);
        _callingCharacters.Remove(toRemove.ToCharacter);
    }

    // STATEFUL, removes from randomized calls
    private Dialogue PopRandomizedCall()
    {
        int index = Random.Range(0, _randomizedCalls.Count);
        DialogueHolder holder = _randomizedCalls[index];
        _randomizedCalls.RemoveAt(index);
        return holder.dialogue;
    }

    private List<DialogueHolder> GetOrderedAvailableDialogue(SingleDayDialogueList dayDiag)
    {
        List<DialogueHolder> returnList = new List<DialogueHolder>();
        foreach (DialogueHolder holder in dayDiag.dialogue)
        {
            bool hasAllTags = true;
            foreach (Tag tag in holder.requiredTags)
            {
                if (!tagsReference.Contains(tag))
                {
                    hasAllTags = false;
                    break;
                }
            }

            if (!hasAllTags)
                continue;

            returnList.Add(holder);
        }

        return returnList;
    }

    private void CheckCalls()
    {
        // Increment backwards
        // So elements can safely be deleted in for loop
        int len = _callList.Count;
        for (int i = len - 1; i > -1; i--)
        {
            CallData dat = _callList[i];
            dat.curTimer -= Time.deltaTime;
            if (dat.curTimer <= 0)
            {
                // Call failed
                strikesLeft--;
                _callList.RemoveAt(i);
            }
        }
    }

    void OnJackRemoved(JackData jackData)
    {
        Switch curSwitch = jackData.SnappedSwitch;
        if (curSwitch == null)
        {
            Debug.LogError("Switch is null, should not be possible in DayManagers OnJackPlaced");
            return;
        }
        Location jackLoc = jackData.SnappedSwitch.locationData;
        _placedJacks.Add(jackData.PlacedJackID);
        JackRemovedInLoc(jackLoc);
        print("Removed event received:" + jackData.ToString());
    }

    void OnJackPlaced(JackData jackData)
    {
        Switch curSwitch = jackData.SnappedSwitch;
        if (curSwitch == null)
        {
            Debug.LogError("Switch is null, should not be possible in DayManagers OnJackPlaced");
            return;
        }
        Location jackLoc = jackData.SnappedSwitch.locationData;
        _placedJacks.Remove(jackData.PlacedJackID);
        JackPlacedInLoc(jackLoc);
        print("Event received:" + jackData.ToString());
    }


    // Jack placed
    // If outgoing caller
    //      play dialogue
    // If incoming caller
    void JackPlacedInLoc(Location loc)
    {
        CharacterInfo characterPlaced = locationManager.GetCharacterFromLocation(loc);

        CallData outgoingDat = CharacterHasOutgoingCall(characterPlaced);
        if (outgoingDat != null)
        {
            // handle dialogue for outgoing call
        }

        CallData incomingDat = CharacterHasOutgoingCall(characterPlaced);
        if (incomingDat != null)
        {

            //incomingDat.fromCharacter

        }
    }

    void JackRemovedInLoc(Location loc)
    {
        CharacterInfo characterRemoved = locationManager.GetCharacterFromLocation(loc);
    }

    CallData CharacterHasIncomingCall(CharacterInfo character)
    {
        foreach (CallData dat in _callList)
        {
            if (character == dat.toCharacter)
            {
                return dat;
            }
        }
        return null;
    }

    CallData CharacterHasOutgoingCall(CharacterInfo character)
    {
        foreach (CallData dat in _callList)
        {
            if (character == dat.fromCharacter)
            {
                return dat;
            }
        }
        return null;
    }



    // Jack placed -> is the player in a call -> find that call info
    // otherwise don't do anything

}
