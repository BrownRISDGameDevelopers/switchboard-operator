using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

enum DayState
{
    WAITING,
    GENERATING_CALLS,
    BLINKY_TIME,
    SHOW_DIALOGUE,
    FAIL_ONCE,
    SUCCESS_ONCE,
    DAY_OVER
}

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
    public delegate void OnStrike(int strikesLeft, bool recharge);
    public static event OnStrike onStrike;

    private List<DialogueHolder> _orderedCalls;

    private float _callBlinkPeriod = -1.0f;
    private float _waitingTime = 5.0f;
    private float _timeToLockIn = 4.0f;

    private Dictionary<int, Location> JackIDToLocation = new Dictionary<int, Location>();

    private Dialogue currentDialogue = null;
    private CharacterInfo currentCharacter = null;
    private Location incomingLocation;
    private Location outgoingLocation;

    private ClockHand _clockHand = null;

    void Start()
    {
        locationManager = FindFirstObjectByType<LocationManager>();
        dialogueUI = FindFirstObjectByType<DialogueUI>();
        _clockHand = FindFirstObjectByType<ClockHand>();
        _switchboard = FindAnyObjectByType<Switchboard>();

        _orderedCalls = GetOrderedAvailableDialogue(currentDay.OrderedCallPool);
        Jack.onJackPlaced += OnJackPlaced;
        Jack.onJackTaken += OnJackRemoved;
        LockInButton.onJackLock += LockIn;
        dialogueUI.OnDialogueDone += OnDialogueDone;

        DoSomeWaiting(3.0f);
    }

    void Update()
    {
        switch (dayState)
        {
            case DayState.WAITING:
                if (_waitingTime > 0)
                {
                    _waitingTime -= Time.deltaTime;
                }
                else
                {
                    dayState = DayState.GENERATING_CALLS;
                }
                break;
            case DayState.GENERATING_CALLS:

                currentDialogue = TryGetNextOrderedDialogue();

                if (currentDialogue)
                {
                    _callBlinkPeriod = 10.0f;
                    dayState = DayState.BLINKY_TIME;
                }
                else
                {
                    dayState = DayState.DAY_OVER;
                }

                break;
            case DayState.BLINKY_TIME:
                GetDialogueLocations();
                if (_callBlinkPeriod < 0)
                {
                    _switchboard.SetSwitchTiming(incomingLocation, 0.0f);
                    dayState = DayState.FAIL_ONCE;
                }
                else
                {
                    _callBlinkPeriod -= Time.deltaTime;
                    _switchboard.SetSwitchTiming(incomingLocation, _callBlinkPeriod);

                    if (JackIDToLocation.ContainsValue(incomingLocation))
                    {
                        dialogueUI.StartDialogueWithData(currentDialogue);
                        currentCharacter = currentDialogue.FromCharacter;

                        _timeToLockIn = 5.0f;
                        _clockHand.rotateClock(0.0f);
                        dayState = DayState.SHOW_DIALOGUE;
                    }
                }

                break;
            case DayState.SHOW_DIALOGUE:
                if (!JackIDToLocation.ContainsValue(incomingLocation))
                {
                    dayState = DayState.FAIL_ONCE;
                    break;
                } else if (GetCurrentlyInDialogue() != null)
                {
                    break;
                }

                if (_timeToLockIn > 0)
                {
                    print("LOCKIN: " + _timeToLockIn);
                    _clockHand.rotateClock(_timeToLockIn / 5.0f);
                    _timeToLockIn -= Time.deltaTime;
                }
                else
                {
                    dayState = DayState.FAIL_ONCE;
                }

                break;
            case DayState.FAIL_ONCE:
                Strike();
                OnCallFail(currentDialogue);
                finish(3.0f);
                break;
            case DayState.SUCCESS_ONCE:
                OnCallCompleteSuccess(currentDialogue);
                finish(3.0f);
                break;
            case DayState.DAY_OVER:
                print("YOU WON!");
                break;
        }
    }

    private void finish(float time)
    {
        dialogueUI.EndEarly();
        _switchboard.SetSwitchTiming(incomingLocation, 0.0f);
        DoSomeWaiting(time);
    }

    private void DoSomeWaiting(float time)
    {
        _waitingTime = time;
        currentDialogue = null;
        print("HOLD UP WAIT A MINUTE");
        dayState = DayState.WAITING;
    }
    private void Strike()
    {
        strikesLeft--;
        onStrike.Invoke(strikesLeft, false);
    }

    private void GetDialogueLocations()
    {
        incomingLocation = locationManager.GetLocationFromCharacter(currentDialogue.FromCharacter);
        outgoingLocation = locationManager.GetLocationFromCharacter(currentDialogue.ToCharacter);
    }

    public void LockIn(int JackSetNumber)
    {
        print("JACKSET: " + JackSetNumber);
        int jack_a_index = JackSetNumber * 2 + 0;
        int jack_b_index = JackSetNumber * 2 + 1;

        Location loc_a;
        Location loc_b;

        bool successA = JackIDToLocation.TryGetValue(jack_a_index, out loc_a);
        bool successB = JackIDToLocation.TryGetValue(jack_b_index, out loc_b);

        if (!successA || !successB)
        {
            print(successA + "....." + successB);
            return;
        }

        bool atob = loc_a.Equals(incomingLocation) && loc_b.Equals(outgoingLocation);
        bool btoa = loc_b.Equals(incomingLocation) && loc_a.Equals(outgoingLocation);

        if (atob || btoa)
        {
            dayState = DayState.SUCCESS_ONCE;         
        }
        else if (loc_a.Equals(incomingLocation) || loc_a.Equals(outgoingLocation) ||
            loc_b.Equals(incomingLocation) || loc_b.Equals(outgoingLocation))
        {
            dayState = DayState.FAIL_ONCE;
        }

        return;
    }

    void OnCallCompleteSuccess(Dialogue dialogue)
    {
        transform.GetComponent<AudioSource>().Play();
        tagsReference.AddTags(dialogue.successTags);
    }

    void OnCallFail(Dialogue dialogue)
    {
        tagsReference.AddTags(dialogue.failureTags);
    }

    public void SetupDayManager(TagsManager newTags, LocationManager locManager, Day day)
    {
        if (day != null) currentDay = day;

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

    private List<DialogueHolder> GetOrderedAvailableDialogue(SingleDayDialogueList dialoguesList)
    {
        List<DialogueHolder> returnList = new List<DialogueHolder>();
        foreach (DialogueHolder holder in dialoguesList.dialogues)
        {
            returnList.Add(holder);
        }
        return returnList;
    }

    private Dialogue TryGetNextOrderedDialogue()
    {
        int count = _orderedCalls.Count;
        print("ORDERED CALLS: " + _orderedCalls.Count);
        if (count > 0)
        {
            Dialogue returnDialogue = _orderedCalls[0].dialogue;
            _orderedCalls.RemoveAt(0);
            return returnDialogue;
        }
        return null;
    }
    public CharacterInfo GetCurrentlyInDialogue()
    {
        return currentCharacter;
    }

    void OnJackRemoved(JackData jackData)
    {
        print("JACK REMOVED: " + jackData.PlacedJackID);
        JackIDToLocation.Remove(jackData.PlacedJackID);
    }

    void OnJackPlaced(JackData jackData)
    {
        Location jackLoc = jackData.SnappedSwitch.locationData;
        print("JACK PLACED: " + jackData.PlacedJackID);
        JackIDToLocation.Add(jackData.PlacedJackID, jackLoc);
    }

    void OnDialogueDone()
    {
        currentCharacter = null;
    }
}
