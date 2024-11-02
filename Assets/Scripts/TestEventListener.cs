using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEventListener : MonoBehaviour
{
    public Switch switchie;
    private float initialTime;
    // Start is called before the first frame update
    void Start()
    {
        Jack.onJackPlaced += JackPlaced;
        Jack.onJackTaken += JackTaken;
        this.initialTime = 10;
    }

    private void JackPlaced(JackData jackData)
    {
        print("Jack Placed: " + jackData.ToString());
    }

    private void JackTaken(JackData jackData)
    {
        print("Jack Taken: " + jackData.ToString());
    }

    void Update(){ 
        switchie.blinkSwitch(initialTime);
        initialTime -= Time.deltaTime;
    }
}
