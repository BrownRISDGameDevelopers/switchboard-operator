using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;

public struct JackData
{
    public int PlacedJackID;
    public Switch SnappedSwitch;
    public bool IsOriginalPosition;
    public override string ToString() => $"Jack: {PlacedJackID} at ({SnappedSwitch.transform.position.x}, {SnappedSwitch.transform.position.y}, {SnappedSwitch.transform.position.z}) or {SnappedSwitch.locationData.Letter}{SnappedSwitch.locationData.Number} which " + (IsOriginalPosition ? "is not" : "is") + " a switch on the board";
}

public class Jack : MonoBehaviour
{
    private Vector3 initialOffset;
    private Vector3 initialPosition;

    public Switchboard switchboard;
    public int jackID;
    public delegate void OnJackPlaced(JackData jackData);
    public static event OnJackPlaced onJackPlaced;
    public Switch jackSwitch;

    public float jackPlacedRange = 2.0f;
    //private CharacterInfo _associatedCharacter;

    //When the mouse is clicked on the collider, set isGettingDragged to true, and defines the initial clicking offset
    void OnMouseDown()
    {
        initialOffset = transform.position - GetMousePosition();
    }

    void OnMouseDrag()
    {
        transform.position = GetMousePosition() + initialOffset;
    }

    //When mouse is released, stop dragging and lock to the nearest switch
    void OnMouseUp()
    {
        Switch closestSwitch = switchboard.GetClosestSwitchPosition(this);
        print("Released");

        if (Vector3.Distance(this.transform.position, closestSwitch.transform.position) > jackPlacedRange)
        {
            this.transform.position = initialPosition;
            return;
        }
        this.transform.position = closestSwitch.transform.position;
        //Event saying that the jack has been placed somewhere & checks if there are listeners
        if (onJackPlaced != null)
        {
            JackData data = new JackData() { PlacedJackID = jackID, SnappedSwitch = closestSwitch, IsOriginalPosition = closestSwitch.transform.position == jackSwitch.transform.position };
            print("Sent event: " + data.ToString());
            onJackPlaced(data);
        }
    }

    //Gets the current mouse position as a Vector3
    UnityEngine.Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(new UnityEngine.Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = jackSwitch.transform.position;
        }
    }

    public void configure(Switch js, int jackid, Switchboard board /*CharacterInfo character*/)
    {
        this.jackSwitch = js;
        transform.position = jackSwitch.transform.position;
        initialPosition = transform.position;
        this.jackID = jackid;
        this.switchboard = board;
        //this._associatedCharacter = character;

    }
}
