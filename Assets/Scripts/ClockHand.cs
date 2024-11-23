using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClockHand : MonoBehaviour
{
    [SerializeField]
    private Transform pivot;
    //private Vector3 rotationPivot;
    // Start is called before the first frame update
    void Start()
    {
        //this.rotationPivot = new Vector3(-4.193f, 1.072f, 0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Rotate clock hand by percentage, where 0-1 is a period
    public void rotateClock(float percentage)
    {
        float angle = (1 - percentage) * 360f;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.back);

        Vector3 offset = rotation * (Vector3.up * 0.27f);

        transform.position = pivot.position + offset;
        transform.rotation = rotation;
        //transform.RotateAround(pivot.position, Vector3.back, 360 * percentage);
    }
}
