using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DayManager : MonoBehaviour
{

    private HashSet<string> tagsReference;
    private LocationManager locationManager;

    public void SetTagsReference(HashSet<string> newTags)
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
        //



    }

    void OnSwitchBoardEvent()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
