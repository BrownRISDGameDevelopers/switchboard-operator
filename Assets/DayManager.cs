using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        Location jackLoc = jackData.SnappedSwitch.locationData;
        print("Event received:" + jackData.ToString());
    }

    // Update is called once per frame
    void Update()
    {

    }



}
