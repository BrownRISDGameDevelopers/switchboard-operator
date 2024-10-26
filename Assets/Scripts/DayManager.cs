using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: we need way for calls to be ignored, and calls need to be on UI
public class DayManager : MonoBehaviour
{

    private HashSet<Tag> tagsReference;
    private Day currentDay;
    private LocationManager locationManager;
    private Switchboard _switchboard;


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

    // Start is called before the first frame update
    void Start()
    {
        // Call upon switch board
        // connect events
        Jack.onJackPlaced += OnJackPlaced;
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

    // Update is called once per frame
    void Update()
    {

    }



}
