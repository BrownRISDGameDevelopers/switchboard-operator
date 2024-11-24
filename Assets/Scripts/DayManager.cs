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


[System.Flags]
enum CallState
{
    INCOMING = 1 << 0,
    SHOWING_DIALOGUE = 1 << 1,
    DIALOGUE_COMPLETE_NO_LOCKIN = 1 << 2,
    LOCKED_IN = 1 << 3,
    ENDED = 1 << 4,
    CANT_IGNORE = ENDED | SHOWING_DIALOGUE,
}

class CallData
{
    public float curTimer;
    public bool dialogueRevealed = false;

    public Dialogue associatedDialogue;

    public CharacterInfo fromCharacter;
    public CharacterInfo toCharacter;

    public CallState state;

    public CallData(CharacterInfo fromChar, CharacterInfo toChar, float timer, Dialogue dialogue, CallState newState)
    {
        curTimer = timer;
        fromCharacter = fromChar;
        toCharacter = toChar;
        associatedDialogue = dialogue;
        state = newState;
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

    public delegate void OnStrike(int strikesLeft, bool recharge);
    public static event OnStrike onStrike;

    public delegate void OnDayEndDelegate();
    public static event OnDayEndDelegate OnDayEnd;


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
    // Locked in, successful, how long to wait until giving the player the W
    private float _postLockInSuccessTime = 5.0f;


    [SerializeField]
    private float _mintuesPerDay = 5.0f;

    private float _secondsLeftInDay;
    // Jackin it 
    private Dictionary<int, Jack> _idToJack = new Dictionary<int, Jack>();
    private HashSet<int> _placedJacks = new HashSet<int>();

    // a "Jackset" is the group of two jacks
    // input jackset id (if 3 pairs, a number 0 to 2) and get the currently held callers
    // held callers data is nullable 
    private Dictionary<int, JackCallersHeldData> _jackSetToHeldCallers = new Dictionary<int, JackCallersHeldData>();


    private CharacterInfo _currentlyInDialogue = null;

    private ClockHand _clockHand = null;

    private int _curCallWeight = 0;
    private int _maxCallWeight = 3;

    void Start()
    {
        locationManager = FindFirstObjectByType<LocationManager>();
        dialogueUI = FindFirstObjectByType<DialogueUI>();
        _clockHand = FindFirstObjectByType<ClockHand>();

        // Of note, this is only called once, so if you get a necessary tag later and then the random thing comes in, may cause an issue
        _randomizedCalls = GetRandomizedAvailableDialogue(currentDay.RandomizedCallPool);
        _orderedCalls = GetOrderedAvailableDialogue(currentDay.OrderedCallPool);
        // Call upon switch board
        // connect events
        Jack.onJackPlaced += OnJackPlaced;
        Jack.onJackTaken += OnJackRemoved;
        LockInButton.onJackLock += LockIn;

        _secondsLeftInDay = _mintuesPerDay * 60.0f;

        if (dialogueUI != null)
            dialogueUI.OnDialogueDone += OnDialogueUIDone;

        SetSwitchboard();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDayEnd();
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

        Debug.Log("CHARACTER PLACED: " + ((characterPlaced != null) ? characterPlaced.CharName : "NULL") + " " + jackSet.ToString());
        // Logic for if the placed jack hit something with a character that has an outgoing call (from character)
        CallData outgoingDat = CharacterHasOutgoingCall(characterPlaced);
        if (outgoingDat != null)
        {
            //outgoingDat.fromCharacter = characterPlaced;
            SetOutgoingForJackSet(jackSet, characterPlaced);

            // If dialogue hasn't been revealed yet (this call wasn't received yet)
            // Play the dialogue and give the player some more time to lock it in
            if (!outgoingDat.dialogueRevealed)
            {
                outgoingDat.curTimer = _postCallReceivedWaitTime;
                // TODO: Actually play dialogue

                if (dialogueUI)
                {
                    _currentlyInDialogue = outgoingDat.fromCharacter;
                    outgoingDat.state = CallState.SHOWING_DIALOGUE;
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
            print("JACK REMOVED OUTGOING NOT NULL");
            // End dialogue if going
            //outgoingDat.fromCharacter = null;
            EndIfCurrentlyInDialogue();
            SetOutgoingForJackSet(jackSet, null);
            OnCallIgnore(outgoingDat);
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
        _jackSetToHeldCallers.TryGetValue(JackSetNumber, out JackCallersHeldData jackHeldData);
        print(jackHeldData);
        print(JackSetNumber);
        print(_jackSetToHeldCallers);

        // Is a valid Jackset

        //Debug.Log("LOCK IN: " + JackSetNumber + " " + jackHeldData != null ? jackHeldData.ToString() : "NULL JACK DATA");

        if (jackHeldData == null)
        {
            print("null jackhelddata in game manager");
            return;
        }

        if (jackHeldData.from == null)
        {
            print("null jack held data from in game manager");
            return;
        }

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

            OnCallFail(outGoingCall);
            Debug.Log("Null Fail!");
            // failure
            return;
        }

        if (outGoingCall.toCharacter != jackHeldData.to)
        {
            Debug.Log("Wrong Character Fail!");
            // Also failgure, but also potential specific character to character tags
            tagsReference.AddTags(outGoingCall.associatedDialogue.GetTagsFromCharacter(jackHeldData.to));
            OnCallFail(outGoingCall);
            return;
        }
        // We've reached the success case

        outGoingCall.state = CallState.LOCKED_IN;
        outGoingCall.curTimer = _postLockInSuccessTime;
        Debug.Log("Success start");
    }

    void OnCallCompleteSuccess(CallData toComplete)
    {
        tagsReference.AddTags(toComplete.associatedDialogue.successTags);
        RemoveCallFromData(toComplete);
    }

    void OnCallFail(CallData toComplete)
    {
        tagsReference.AddTags(toComplete.associatedDialogue.failureTags);
        RemoveCallFromData(toComplete);
        Strike();
    }

    void OnCallIgnore(CallData toComplete)
    {
        tagsReference.AddTags(toComplete.associatedDialogue.ignoreTags);
        RemoveCallFromData(toComplete);
        Strike();
    }

    void OnDialogueUIDone()
    {
        CallData dat = CharacterHasOutgoingCall(_currentlyInDialogue);
        if (dat != null)
        {
            if ((dat.state & CallState.SHOWING_DIALOGUE) != 0)
            {
                dat.state = CallState.DIALOGUE_COMPLETE_NO_LOCKIN;
                dat.curTimer = _postCallReceivedWaitTime;
            }

        }
        _currentlyInDialogue = null;
    }


    private void EndIfCurrentlyInDialogue()
    {
        if (_currentlyInDialogue != null && dialogueUI != null)
        {
            dialogueUI.EndEarly();
            _currentlyInDialogue = null;
        }
    }
    public CharacterInfo GetCurrentlyInDialogue()
    {
        return _currentlyInDialogue;
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

        if (_curCallWeight >= _maxCallWeight)
        {
            _callNextTimer = 3.0f;
            return;
        }
        Dialogue newCall = null;



        // TODO: How do we want to do this?
        // rn randomly get random or ordered calls, if ordered call is null, get random call
        if (_orderedCalls.Count == 0 || (_curOrderedCall > 0 && Random.Range(0, 100) < 50.0f && _randomizedCalls.Count > 0))
        {
            newCall = PopRandomizedCall();
        }
        else
        {
            newCall = TryGetNextOrderedDialogue();
            if (newCall == null)
            {
                newCall = PopRandomizedCall();
            }
        }

        print("time");
        if (newCall == null)
        {
            _callNextTimer = Random.Range(_callAddTimeMin, _callAddTimeMax);
            return;
        }

        print(_orderedCalls.Count.ToString() + " diag " + newCall.ToCharacter.CharName);
        // ADD RELEVANT INFO
        CallData newCallDat = new CallData(newCall.FromCharacter, newCall.ToCharacter, _incomingCallReceivedWaitTime, newCall, CallState.INCOMING);
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
        if (day != null)
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
        _curCallWeight += toAdd.associatedDialogue.CallWeight;
    }

    private void RemoveCallFromData(CallData toRemove)
    {
        _callList.Remove(toRemove);
        _callingCharacters.Remove(toRemove.fromCharacter);
        _callingCharacters.Remove(toRemove.toCharacter);
        _curCallWeight -= toRemove.associatedDialogue.CallWeight;
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

            // If the next ordered call has a character, just have to return nothing so nothing gets messed up
            if (CharacterInCall(currentOrderedCall.FromCharacter) || CharacterInCall(currentOrderedCall.ToCharacter))
            {
                return null;
            }

            if (currentOrderedCall != null && DialogueHasValidTags(currentOrderedCall))
            {
                _curOrderedCall++;
                return currentOrderedCall;
            }
            _curOrderedCall++;
        }
        return null;
    }


    private bool DialogueHasValidTags(Dialogue dialogue)
    {

        if (tagsReference == null)
        {
            print("tags reference null (dialoguehasvalidtags)");
            return true;
        }
        if (dialogue == null)
        {
            print("dialogue null (dialoguehasvalidtags)");
            return true;
        }
        bool hasAllTags = tagsReference.HasAllTags(dialogue.requiredTags);
        bool hasDisallowedTags = tagsReference.HasNoTags(dialogue.disallowedTags);
        return hasAllTags && !hasDisallowedTags;
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


    private void CheckDayEnd()
    {
        _secondsLeftInDay -= Time.deltaTime;

        if (_clockHand != null)
            _clockHand.rotateClock(_secondsLeftInDay / (_mintuesPerDay * 60.0f));

        if (_secondsLeftInDay > 0 &&
             (_curOrderedCall < _orderedCalls.Count || _randomizedCalls.Count > 0 || _callList.Count > 0))
            return;

        OnDayEnd?.Invoke();
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

            bool canIgnore = (dat.state & (CallState.CANT_IGNORE)) == 0;

            if (canIgnore)
                _switchboard.SetSwitchTiming(loc, dat.curTimer);
            else
                _switchboard.SetSwitchTiming(loc, 0);


            if (dat.curTimer <= 0)
            {
                if ((dat.state & (CallState.LOCKED_IN)) != 0)
                {
                    OnCallCompleteSuccess(dat);
                    Debug.Log("Success!");
                }
                else if (canIgnore)
                {
                    Debug.Log("Call ignored!");
                    _switchboard.SetSwitchTiming(loc, 0); // TODO, may want another sprite or other indicator of ignoring
                    OnCallIgnore(dat);
                }
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
        if (character != null)
            Debug.Log("OUT GOING JACK SET " + jackSet + " " + character.CharName);

        if (!_jackSetToHeldCallers.ContainsKey(jackSet))
        {
            _jackSetToHeldCallers.Add(jackSet, new JackCallersHeldData());
        }
        _jackSetToHeldCallers.TryGetValue(jackSet, out JackCallersHeldData dat);
        dat.from = character;
        _jackSetToHeldCallers[jackSet] = dat;
    }

    void SetIncomingForJackSet(int jackSet, CharacterInfo character)
    {
        if (!_jackSetToHeldCallers.ContainsKey(jackSet))
        {
            _jackSetToHeldCallers.Add(jackSet, new JackCallersHeldData());
        }
        _jackSetToHeldCallers.TryGetValue(jackSet, out JackCallersHeldData dat);
        dat.to = character;
        _jackSetToHeldCallers[jackSet] = dat;
    }

    CallData CharacterHasIncomingCall(CharacterInfo character)
    {
        if (character == null)
            return null;
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
        if (character == null)
            return null;
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
