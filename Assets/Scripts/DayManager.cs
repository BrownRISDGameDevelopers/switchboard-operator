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


enum CallState
{
    INCOMING = 0,
    ENDED = 1,

    LOCKED_IN = 2,

}

class CallData
{
    public float curTimer;
    public bool dialogueRevealed = false;

    public Dialogue associatedDialogue;

    public CharacterInfo fromCharacter;
    public CharacterInfo toCharacter;

    public CallData(CharacterInfo fromChar, CharacterInfo toChar, float timer, Dialogue dialogue)
    {
        curTimer = timer;
        fromCharacter = fromChar;
        toCharacter = toChar;
        associatedDialogue = dialogue;
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
// TODO: Potential edge case, what if a jack is locked into a character/location that doesn't have a call, then a dialogue comes in with a call for that character?
public class DayManager : MonoBehaviour
{

    [SerializeField]
    private Day currentDay;

    private TagsManager tagsReference;
    private LocationManager locationManager;
    private Switchboard _switchboard;

    private DialogueUI dialogueUI;

    public int strikesLeft = 3;

    // For incoming call logic
    // THIS LIST IS IMPORTANT
    // All incoming or current calls on it
    private List<CallData> _callList = new List<CallData>();

    // All characters current in an incoming or current call
    private HashSet<CharacterInfo> _callingCharacters = new HashSet<CharacterInfo>();

    // List of potential calls
    // Initialized in Start, then popped off when randomized dialogue needed
    private List<DialogueHolder> _randomizedCalls;
    private List<DialogueHolder> _orderedCalls;
    private int _curOrderedCall = 0;
    // All previously done dialogues (not used yet) 
    private HashSet<Dialogue> _previousCalls = new HashSet<Dialogue>();


    // Randomized call timing 
    // Time between 
    //private float _callTimeoutTime = 1.0f;
    // Minimum and maximum time a new randomized call will come in
    private float _callAddTimeMin = 1.0f;
    private float _callAddTimeMax = 10.0f;
    // State, data for timing when next call will be  
    private float _callNextTimer = 5.0f;

    // A new call is incoming, here's how long you have to answer it
    private float _incomingCallReceivedWaitTime = 10.0f;
    // Once answered, heres how long you have to lock it in with the right thang
    private float _postCallReceivedWaitTime = 10.0f;

    public delegate void OnStrike(int strikesLeft, bool recharge);
    public static event OnStrike onStrike;

    // Jackin it 
    private Dictionary<int, Jack> _idToJack = new Dictionary<int, Jack>();
    private HashSet<int> _placedJacks = new HashSet<int>();

    // a "Jackset" is the group of two jacks
    // input jackset id (if 3 pairs, a number 0 to 2) and get the currently held callers
    // held callers data is nullable 
    private Dictionary<int, JackCallersHeldData> _jackSetToHeldCallers = new Dictionary<int, JackCallersHeldData>();

    void Start()
    {
        locationManager = FindFirstObjectByType<LocationManager>();
        dialogueUI = FindFirstObjectByType<DialogueUI>();

        // Of note, this is only called once, so if you get a necessary tag later and then the random thing comes in, may cause an issue
        _randomizedCalls = GetRandomizedAvailableDialogue(currentDay.RandomizedCallPool);
        _orderedCalls = GetOrderedAvailableDialogue(currentDay.OrderedCallPool);
        // Call upon switch board
        // connect events
        Jack.onJackPlaced += OnJackPlaced;
        Jack.onJackTaken += OnJackRemoved;
        LockInButton.onJackLock += LockIn;
        SetSwitchboard();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForIncomingCalls();
        CheckCalls();
    }

    // Jack placed
    // If outgoing caller
    //      play dialogue
    // If incoming caller
    void JackPlacedInLoc(Location loc, int jackId)
    {
        CharacterInfo characterPlaced = locationManager.GetCharacterFromLocation(loc);
        int jackSet = GetAssociatedJackSet(jackId);

        Debug.Log("CHARACTER PLACED: " + ((characterPlaced != null) ? characterPlaced.CharName : "NULL"));
        // Logic for if the placed jack hit something with a character that has an outgoing call (from character)
        CallData outgoingDat = CharacterHasOutgoingCall(characterPlaced);
        if (outgoingDat != null)
        {
            outgoingDat.fromCharacter = characterPlaced;
            SetOutgoingForJackSet(jackSet, characterPlaced);

            // If dialogue hasn't been revealed yet (this call wasn't received yet)
            // Play the dialogue and give the player some more time to lock it in
            if (!outgoingDat.dialogueRevealed)
            {
                outgoingDat.curTimer = _postCallReceivedWaitTime;
                // TODO: Actually play dialogue

                if (dialogueUI)
                {
                    dialogueUI.StartDialogueWithData(outgoingDat.associatedDialogue);
                }
                outgoingDat.dialogueRevealed = true;
            }
        }

        // Logic for if the placed jack hit something with a character that has an incoming call (to character)
        CallData incomingDat = CharacterHasIncomingCall(characterPlaced);
        if (incomingDat != null)
        {
            incomingDat.toCharacter = characterPlaced;
            SetIncomingForJackSet(jackSet, characterPlaced);
        }
    }

    void JackRemovedInLoc(Location loc, int jackId)
    {
        CharacterInfo characterRemoved = locationManager.GetCharacterFromLocation(loc);

        int jackSet = GetAssociatedJackSet(jackId);

        CallData outgoingDat = CharacterHasOutgoingCall(characterRemoved);
        if (outgoingDat != null)
        {
            outgoingDat.fromCharacter = null;
            SetOutgoingForJackSet(jackSet, null);
        }

        CallData incomingDat = CharacterHasIncomingCall(characterRemoved);
        if (incomingDat != null)
        {
            incomingDat.toCharacter = null;
            SetIncomingForJackSet(jackSet, null);
        }
    }

    public void LockIn(int JackSetNumber)
    {
        JackCallersHeldData jackHeldData = _jackSetToHeldCallers.GetValueOrDefault(JackSetNumber);

        Debug.Log("LOCK IN: " + JackSetNumber.ToString() + " " + jackHeldData.ToString());
        // Is a valid Jackset

        if (jackHeldData == null)
            return;

        if (jackHeldData.from == null)
            return;

        // Cases
        // Standard, jack is connected to correct incoming and outgoing caller
        //      Start jack timer, when it ends, successful call, add tags
        // Nothing, jack is not connected to any callers, do nothing
        // Connected to just an outgoing caller
        //      Fail, add failure tags
        // Connected to an outgoing caller with incorrect receiver
        //      Fail, strike, but add other tags
        // We don't care about the case where its just connected to an incoming caller

        // This will not be null because its only set if the jack has an outgoing case, but gonna check anyways
        CallData outGoingCall = CharacterHasOutgoingCall(jackHeldData.from);
        if (outGoingCall == null)
        {
            Debug.LogError("Null that should be impossible in day manager");
            return;
        }

        if (jackHeldData.to == null)
        {
            tagsReference.AddTags(outGoingCall.associatedDialogue.failureTags);
            RemoveCallFromData(outGoingCall);
            Strike();
            Debug.Log("Null Fail!");
            // failure
            return;
        }

        if (outGoingCall.toCharacter != jackHeldData.to)
        {
            Debug.Log("Wrong Character Fail!");
            // Also failgure, but also potential specific character to character tags
            tagsReference.AddTags(outGoingCall.associatedDialogue.failureTags);
            tagsReference.AddTags(outGoingCall.associatedDialogue.GetTagsFromCharacter(jackHeldData.to));
            RemoveCallFromData(outGoingCall);
            Strike();
            return;
        }
        // We've reached the success case
        tagsReference.AddTags(outGoingCall.associatedDialogue.successTags);
        RemoveCallFromData(outGoingCall);
        Debug.Log("Success!");
    }


    private void Strike()
    {
        strikesLeft--;
        onStrike.Invoke(strikesLeft, false);
    }

    private void CheckForIncomingCalls()
    {
        _callNextTimer -= Time.deltaTime;
        if (_callNextTimer > 0)
        {
            return;
        }
        Dialogue newCall = null;


        newCall = TryGetNextOrderedDialogue();
        // TODO: How do we want to do this?
        // rn randomly get random or ordered calls, if ordered call is null, get random call
        /*if (Random.Range(0, 100) < 50.0f && _randomizedCalls.Count > 0)
        {
            newCall = PopRandomizedCall();
        }
        else
        {
            if (newCall == null)
            {
                newCall = PopRandomizedCall();
            }
        }*/


        print("time");
        if (newCall == null)
            return;

        print(_orderedCalls.Count.ToString() + " diag " + newCall.ToCharacter.CharName);
        // ADD RELEVANT INFO
        CallData newCallDat = new CallData(newCall.FromCharacter, newCall.ToCharacter, _incomingCallReceivedWaitTime, newCall);
        AddCalltoData(newCallDat);

        Debug.Log(" NEW DIALOGUE " + newCallDat.fromCharacter.CharName + " TO: " + newCallDat.toCharacter.CharName);
        Location loc1 = locationManager.GetLocationFromCharacter(newCallDat.fromCharacter);
        Location loc2 = locationManager.GetLocationFromCharacter(newCallDat.toCharacter);
        Debug.Log(" NEW LOC " + loc1.ToString() + " TO: " + loc2.ToString());

        _callNextTimer = Random.Range(_callAddTimeMin, _callAddTimeMax);
    }

    // nullable
    private int GetAssociatedJackSet(Jack jack)
    {
        return GetAssociatedJackSet(jack.jackID);
    }

    private int GetAssociatedJackSet(int jackID)
    {
        return jackID / 2;//jackID % 2 == 0 ? jackID/2 : jackID - 1;
    }

    public void SetupDayManager(TagsManager newTags, LocationManager locManager, Day day)
    {
        currentDay = day;
        tagsReference = newTags;
        locationManager = locManager;
    }

    public void SetTagsReference(TagsManager newTags)
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
        if (_switchboard)
            return;

        Jack[] jacks = _switchboard.GetJacks();
        foreach (Jack jack in jacks)
        {
            _idToJack.Add(jack.jackID, jack);
        }
    }





    private void AddCalltoData(CallData toAdd)
    {
        _callList.Add(toAdd);
        _callingCharacters.Add(toAdd.fromCharacter);
        _callingCharacters.Add(toAdd.toCharacter);
    }

    private void RemoveCallFromData(CallData toRemove)
    {
        _callList.Remove(toRemove);
        _callingCharacters.Remove(toRemove.fromCharacter);
        _callingCharacters.Remove(toRemove.toCharacter);
    }

    // STATEFUL, removes from randomized calls
    private Dialogue PopRandomizedCall()
    {
        if (_randomizedCalls.Count <= 0)
            return null;

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
            returnList.Add(holder);
        }
        return returnList;
    }

    private Dialogue TryGetNextOrderedDialogue()
    {
        Dialogue currentOrderedCall = null;
        while (_curOrderedCall < _orderedCalls.Count)
        {
            currentOrderedCall = _orderedCalls[_curOrderedCall].dialogue;

            if (currentOrderedCall != null && DialogueHasValidTags(currentOrderedCall))
                return currentOrderedCall;
            _curOrderedCall++;
        }
        return null;
    }


    private bool DialogueHasValidTags(Dialogue dialogue)
    {
        bool hasAllTags = tagsReference.HasAllTags(dialogue.requiredTags);
        bool hasDisallowedTags = tagsReference.HasNoTags(dialogue.disallowedTags);
        return true;//hasAllTags && !hasDisallowedTags;
    }

    private List<DialogueHolder> GetRandomizedAvailableDialogue(SingleDayDialogueList dayDiag)
    {
        List<DialogueHolder> returnList = new List<DialogueHolder>();
        foreach (DialogueHolder holder in dayDiag.dialogue)
        {
            if (DialogueHasValidTags(holder.dialogue))
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

            Location loc = locationManager.GetLocationFromCharacter(dat.fromCharacter);
            _switchboard.SetSwitchTiming(loc, dat.curTimer);
            if (dat.curTimer <= 0)
            {
                Debug.Log("Call ignored!");
                _switchboard.SetSwitchTiming(loc, 0); // TODO, may want another sprite or other indicator of ignoring
                // Call ignored
                tagsReference.AddTags(dat.associatedDialogue.ignoreTags);
                Strike();
                RemoveCallFromData(dat);
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
        _placedJacks.Remove(jackData.PlacedJackID);
        JackRemovedInLoc(jackLoc, jackData.PlacedJackID);
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
        _placedJacks.Add(jackData.PlacedJackID);
        JackPlacedInLoc(jackLoc, jackData.PlacedJackID);
        print("Event received:" + jackData.ToString());
    }

    void SetOutgoingForJackSet(int jackSet, CharacterInfo character)
    {
        Debug.Log("OUT GOING JACK SET " + jackSet + " " + character.CharName);
        if (!_jackSetToHeldCallers.ContainsKey(jackSet))
        {
            _jackSetToHeldCallers.Add(jackSet, new JackCallersHeldData());
        }
        _jackSetToHeldCallers.TryGetValue(jackSet, out JackCallersHeldData dat);
        dat.from = character;
    }

    void SetIncomingForJackSet(int jackSet, CharacterInfo character)
    {
        if (!_jackSetToHeldCallers.ContainsKey(jackSet))
        {
            _jackSetToHeldCallers.Add(jackSet, new JackCallersHeldData());
        }
        _jackSetToHeldCallers.TryGetValue(jackSet, out JackCallersHeldData dat);
        dat.to = character;
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

    bool CharacterInCall(CharacterInfo character)
    {
        return _callingCharacters.Contains(character);
    }
    // Jack placed -> is the player in a call -> find that call info
    // otherwise don't do anything

}
