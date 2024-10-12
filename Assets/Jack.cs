using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public struct JackData{
    public int PlacedID;
    public UnityEngine.Vector3 SelectedOutlet;
    public JackData(int placedId, UnityEngine.Vector3 selectedOutlet){
        PlacedID = placedId;
        SelectedOutlet = selectedOutlet;
    }
    public override string ToString() => $"Jack: {PlacedID} at ({SelectedOutlet.x}, {SelectedOutlet.y}, {SelectedOutlet.z},)";
}

public class Jack : MonoBehaviour
{
    private UnityEngine.Vector3 initialOffest;
    public UnityEngine.Vector3 originalPosition;
    public Switchboard switchboard;
    public int jackID;
    public delegate void OnJackPlaced(JackData jackData);
    public static event OnJackPlaced onJackPlaced;
    
    void Start(){
        transform.position = originalPosition;
    }

    //When the mouse is clicked on the collider, set isGettingDragged to true, and defines the initial clicking offset
    void OnMouseDown(){
        initialOffest = transform.position - GetMousePosition();
    }

    void OnMouseDrag(){
        transform.position = GetMousePosition() + initialOffest;
    }
    
    //When mouse is released, stop dragging and lock to the nearest outlet point
    void OnMouseUp(){
        UnityEngine.Vector3 closestOutlet = switchboard.GetClosestOutlet(transform.position, originalPosition);
        transform.position = closestOutlet;
        print("Released");

        //Event saying that the jack has been placed somewhere & checks if there are listeners
        if (onJackPlaced != null){
            JackData data = new JackData(jackID, closestOutlet);
            print("Sent event: " + data.ToString());
            onJackPlaced(data);
        }
    }

    //Gets the current mouse position as a Vector3
    UnityEngine.Vector3 GetMousePosition(){
        return Camera.main.ScreenToWorldPoint(new UnityEngine.Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.R)){
            transform.position = originalPosition;
        }
    }

    public void configure(UnityEngine.Vector3 originalP, int jackid, Switchboard board){
        this.originalPosition = originalP;
        transform.position = originalPosition;
        this.jackID = jackid;
        this.switchboard = board;

    }
}
