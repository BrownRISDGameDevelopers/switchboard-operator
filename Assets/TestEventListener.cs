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
    }

    private void JackPlaced(JackData jackData)
    {
        print("Event received:" + jackData.ToString());
    }

}
