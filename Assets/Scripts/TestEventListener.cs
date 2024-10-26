using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEventListener : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Jack.onJackPlaced += JackPlaced;
        Jack.onJackTaken += JackTaken;
    }

    private void JackPlaced(JackData jackData)
    {
        print("Jack Placed: " + jackData.ToString());
    }

    private void JackTaken(JackData jackData)
    {
        print("Jack Taken: " + jackData.ToString());
    }

}
