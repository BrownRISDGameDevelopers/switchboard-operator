using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

enum DayState
{
    GENERATING_CALLS,
    BLINKY_TIME,
    SHOW_DIALOGUE,
    FAIL_ONCE,
    DAY_OVER
}

enum CallState
{
    INCOMING = 1,
    SHOWING_DIALOGUE = 2,
    DIALOGUE_COMPLETE_NO_LOCKIN = 3,
    LOCKED_IN = 4,
    ENDED = 5,
    CANT_IGNORE = 6,
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
    [SerializeField]
    private Day currentDay;

    private TagsManager tagsReference;
    private LocationManager locationManager;
    private Switchboard _switchboard;

    private DialogueUI dialogueUI;
    private DayState dayState;

    public int strikesLeft = 3;

    private List<CallData> _callList = new List<CallData>();

    private HashSet<CharacterInfo> _callingCharacters = new HashSet<CharacterInfo>();

    private List<DialogueHolder> _orderedCalls;
    private int _curOrderedCall = 0;

    private float _callAddTimeMin = 1.0f;
    private float _callAddTimeMax = 10.0f;

    private float _callBlinkPeriod = -1.0f;
    private float _callNextTimer = 15.0f;

    // A new call is incoming, here's how long you have to answer it
    private float _incomingCallReceivedWaitTime = 10.0f;
    // Once answered, heres how long you have to lock it in with the right thang
    private float _postCallReceivedWaitTime = 10.0f;
    // Locked in, successful, how long to wait until giving the player the W
    private float _postLockInSuccessTime = 5.0f;

    private Dictionary<int, Jack> _idToJack = new Dictionary<int, Jack>();
    private HashSet<int> _placedJacks = new HashSet<int>();

    // a "Jackset" is the group of two jacks
    // input jackset id (if 3 pairs, a number 0 to 2) and get the currently held callers
    // held callers data is nullable 
    private Dictionary<int, JackCallersHeldData> _jackSetToHeldCallers = new Dictionary<int, JackCallersHeldData>();

    private Dialogue currentDialogue = null;
    private Location incomingLocation;
    private Location outgoingLocation;

    private ClockHand _clockHand = null;

    void Start()
    {
        locationManager = FindFirstObjectByType<LocationManager>();
        dialogueUI = FindFirstObjectByType<DialogueUI>();
        _clockHand = FindFirstObjectByType<ClockHand>();

        _orderedCalls = GetOrderedAvailableDialogue(currentDay.OrderedCallPool);

        Jack.onJackPlaced += OnJackPlaced;
        Jack.onJackTaken += OnJackRemoved;
        LockInButton.onJackLock += LockIn;

        if (dialogueUI != null)
		{
            dialogueUI.OnDialogueDone += OnDialogueUIDone;
		}

        SetSwitchboard();

        dayState = DayState.GENERATING_CALLS;
    }

    void Update()
    {
        switch (dayState)
        {
            case DayState.GENERATING_CALLS:

                if (_curOrderedCall < _orderedCalls.Count)
                {
                    currentDialogue = TryGetNextOrderedDialogue();
                    _curOrderedCall++;

                    _callBlinkPeriod = 10.0f;
                    dayState = DayState.BLINKY_TIME;
                }
                else
                {
                    dayState = DayState.DAY_OVER;
                }

                break;
            case DayState.BLINKY_TIME:
                Location loc = locationManager.GetLocationFromCharacter(currentDialogue.FromCharacter);

                if (_callBlinkPeriod < 0)
                {
                    _switchboard.SetSwitchTiming(loc, 0.0f);
                    dayState = DayState.FAIL_ONCE;
                }
                else
                {
                    _callBlinkPeriod -= Time.deltaTime;
                    _switchboard.SetSwitchTiming(loc, _callBlinkPeriod);
                }

                break;
            case DayState.SHOW_DIALOGUE:

                break;
            case DayState.FAIL_ONCE:
                // Strike
                // Bang bell
                break;
            case DayState.DAY_OVER:
                print("YOU WON!");
                break;
        }
    }

    private int GetAssociatedJackSet(int jackID)
    {
        return jackID / 2;
    }

    void JackPlacedInLoc(Location loc, int jackId)
    {
        CharacterInfo characterPlaced = locationManager.GetCharacterFromLocation(loc);
        int jackSet = GetAssociatedJackSet(jackId);

        if (loc.Equals(incomingLocation) && dayState != DayState.SHOW_DIALOGUE)
        {
            dayState = DayState.SHOW_DIALOGUE;
        }
        else if (loc.Equals(outgoingLocation) && dayState == DayState.SHOW_DIALOGUE)
        {
            
        }

        return;
    }

    void JackRemovedInLoc(Location loc, int jackId)
    {
        CharacterInfo characterRemoved = locationManager.GetCharacterFromLocation(loc);

        int jackSet = GetAssociatedJackSet(jackId);

        CallData outgoingDat = CharacterHasOutgoingCall(characterRemoved);
        if (outgoingDat != null)
        {
            // End dialogue if going
            //outgoingDat.fromCharacter = null;
            SetOutgoingForJackSet(jackSet, null);
            EndIfCurrentlyInDialogue();
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
        JackCallersHeldData jackHeldData = _jackSetToHeldCallers.GetValueOrDefault(JackSetNumber);

        if (jackHeldData == null)
            return;

        if (jackHeldData.from == null)
            return;

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
        // TODO: how to wait n such?
        //
        outGoingCall.state = CallState.LOCKED_IN;
        outGoingCall.curTimer = _postLockInSuccessTime;
        print("CALLSTATE::Success!");
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
    }

    void OnCallIgnore(CallData toComplete)
    {
        tagsReference.AddTags(toComplete.associatedDialogue.ignoreTags);
        RemoveCallFromData(toComplete);
    }

    void OnDialogueUIDone()
    {
        CallData dat = CharacterHasOutgoingCall(_currentlyInDialogue);
        if (dat != null)
        {
            if (dat.state == CallState.SHOWING_DIALOGUE)
            {
                dat.state = CallState.DIALOGUE_COMPLETE_NO_LOCKIN;
                dat.curTimer = _postCallReceivedWaitTime;
            }

        }
        _currentlyInDialogue = null;
    }

    private void CheckForIncomingCalls()
    {
        _callNextTimer -= Time.deltaTime;
        if (_callNextTimer > 0)
        {
            return;
        }

        
        
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


    private List<DialogueHolder> GetOrderedAvailableDialogue(SingleDayDialogueList dialoguesList)
    {
        List<DialogueHolder> returnList = new List<DialogueHolder>();
        foreach (DialogueHolder holder in dialoguesList.dialogues)
        {
            print(holder);
            returnList.Add(holder);
        }
        return returnList;
    }

    private Dialogue TryGetNextOrderedDialogue()
    {
        return _orderedCalls[_curOrderedCall].dialogue;
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

            bool canIgnore = !(dat.state == CallState.ENDED || dat.state == CallState.SHOWING_DIALOGUE);

            if (canIgnore)
            {
                _switchboard.SetSwitchTiming(loc, dat.curTimer);
            }    
            else
            {
                _switchboard.SetSwitchTiming(loc, 0);
            }

            if (dat.curTimer <= 0)
            {
                print(dat.curTimer);
                if (dat.state == CallState.LOCKED_IN)
                {
                    OnCallCompleteSuccess(dat);
                    return;
                }

                if (canIgnore)
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
        Location jackLoc = jackData.SnappedSwitch.locationData;

        _placedJacks.Remove(jackData.PlacedJackID);

        JackRemovedInLoc(jackLoc, jackData.PlacedJackID);
    }

    void OnJackPlaced(JackData jackData)
    {
        Location jackLoc = jackData.SnappedSwitch.locationData;

        _placedJacks.Add(jackData.PlacedJackID);

        JackPlacedInLoc(jackLoc, jackData.PlacedJackID);
    }

    void SetOutgoingForJackSet(int jackSet, CharacterInfo character)
    {
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
}
