using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClockHand : MonoBehaviour
{
    private Vector3 rotationPivot;
    // Start is called before the first frame update
    void Start()
    {
        this.rotationPivot = new Vector3(-4.193f, 1.072f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Rotate clock hand by percentage, where 0-1 is a period
    public void rotateClock(float percentage){
        transform.RotateAround(this.rotationPivot, Vector3.back, 360*percentage);
    }
}
