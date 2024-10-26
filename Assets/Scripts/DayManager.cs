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

struct CallData
{
    float curTimer;
    CharacterInfo fromCharacter;
    CharacterInfo toCharacter;
}


// TODO: we need way for calls to be ignored, and calls need to be on UI
public class DayManager : MonoBehaviour
{

    private HashSet<Tag> tagsReference;
    private Day currentDay;
    private LocationManager locationManager;
    private Switchboard _switchboard;



    // For incoming call logic
    private List<CallData> _callList = new List<CallData>();
    private float _callTimeoutTime;
    private float _callAddTimeMin;
    private float _callAddTimeMax;
    private float _callNextTimer;

    private HashSet<CharacterInfo> _callingCharacters = new HashSet<CharacterInfo>();

    // Update is called once per frame
    void Update()
    {

    }


    private void CheckCalls()
    {


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
    }

    // Start is called before the first frame update
    void Start()
    {
        // Call upon switch board
        // connect events
        Jack.onJackPlaced += OnJackPlaced;
        SetSwitchboard();

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
        print("Event received:" + jackData.ToString());
    }

}
