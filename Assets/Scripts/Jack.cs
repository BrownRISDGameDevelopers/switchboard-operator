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
    [SerializeField] AudioSource SFX_plug_in;
    [SerializeField] AudioSource SFX_plug_out;
    private Vector3 initialOffset;
    private Vector3 initialPosition;

    public Switchboard switchboard;
    public int jackID;
    public delegate void OnJackPlaced(JackData jackData);
    public static event OnJackPlaced onJackPlaced;
    public delegate void OnJackTaken(JackData jackData);
    public static event OnJackTaken onJackTaken;
    public Switch jackSwitch;
    public float jackPlacedRange = 2.0f;

    //Find a better implementation of this
    public Sprite baseSprite;
    public Sprite dragSprite;
    public Sprite placedSprite;
    public Sprite blueBaseSprite;
    public Sprite blueDragSprite;
    public Sprite bluePlacedSprite;
    public Sprite greenBaseSprite;
    public Sprite greenDragSprite;
    public Sprite greenPlacedSprite;
    public Sprite redBaseSprite;
    public Sprite redDragSprite;
    public Sprite redPlacedSprite;

    //private CharacterInfo _associatedCharacter;
    private LineRenderer _lineRenderer;

    private SpriteRenderer _baseSpriteRenderer;
    private SpriteRenderer _dragSpriteRenderer;
    private SpriteRenderer _placedSpriteRenderer;
    private float holeOffset = 0.15f;
    private float wireOffset = 0.5f;


    public LineRenderer wireLineRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        _baseSpriteRenderer = transform.Find("baseJack").GetComponent<SpriteRenderer>();
        _dragSpriteRenderer = transform.Find("dragJack").GetComponent<SpriteRenderer>();
        _placedSpriteRenderer = transform.Find("placedJack").GetComponent<SpriteRenderer>();

        if (_baseSpriteRenderer == null || _baseSpriteRenderer == null || _baseSpriteRenderer == null)
        {
            Debug.LogError("No sprite render component in switch");
            return;
        }
        _baseSpriteRenderer.gameObject.SetActive(true);
        _dragSpriteRenderer.gameObject.SetActive(false);
        _placedSpriteRenderer.gameObject.SetActive(false);

        wireOffset += holeOffset;
        transform.Find("jackWirePointer").position += Vector3.up*holeOffset;

        wireLineRenderer.endColor = Color.clear;
        wireLineRenderer.startColor = Color.clear;
    }
    //When the mouse is clicked on the collider, set isGettingDragged to true, and defines the initial clicking offset

    void OnMouseDown()
    {

        wireLineRenderer.endColor = Color.white;
        wireLineRenderer.startColor = Color.white;
        transform.Find("jackWirePointer").position += Vector3.down*wireOffset;
        Cursor.visible = false;

        // THIS COULD CAUSE AN ERROR IF THE NEAREST SWITCH IS ALSO CLOSE TO THE SAME SWITCH THAT ANOTHER THING IS IN 
        initialOffset = transform.position - GetMousePosition();
        Switch closestSwitch = switchboard.GetClosestSwitchPosition(this, 0f);

        transform.position = closestSwitch.transform.position;
        closestSwitch.isTaken = false;

        SFX_plug_out.Play();

        //Event saying that the jack has been placed somewhere & checks if there are listeners
        if (onJackTaken != null)
        {
            JackData data = new JackData() { PlacedJackID = jackID, SnappedSwitch = closestSwitch, IsOriginalPosition = closestSwitch.transform.position == jackSwitch.transform.position };
            onJackTaken(data);
        }
    }

    void OnMouseDrag()
    {
        _baseSpriteRenderer.gameObject.SetActive(false);
        _dragSpriteRenderer.gameObject.SetActive(true);
        _placedSpriteRenderer.gameObject.SetActive(false);

        Vector3 mousePos = GetMousePosition();
        transform.position = mousePos + initialOffset;
    }

    //When mouse is released, stop dragging and lock to the nearest switch
    void OnMouseUp()
    {
        transform.Find("jackWirePointer").position += Vector3.up*wireOffset;
        Cursor.visible = true;

        Switch closestSwitch = switchboard.GetClosestSwitchPosition(this, 0.6f);

        if (Vector3.Distance(transform.position, closestSwitch.transform.position) > jackPlacedRange
            || closestSwitch.isTaken)
        {

            wireLineRenderer.endColor = Color.clear;
            wireLineRenderer.startColor = Color.clear;

            transform.position = initialPosition;
            _baseSpriteRenderer.gameObject.SetActive(true);
            _dragSpriteRenderer.gameObject.SetActive(false);
            _placedSpriteRenderer.gameObject.SetActive(false);

            ScreenShakeCamera.TryAddShake(Constants.JACK_OFF_SHAKE);
            return;
        }

        transform.position = closestSwitch.transform.position;
        closestSwitch.isTaken = true;

        _baseSpriteRenderer.gameObject.SetActive(false);
        _dragSpriteRenderer.gameObject.SetActive(false);
        _placedSpriteRenderer.gameObject.SetActive(true);

        SFX_plug_in.Play();
        ScreenShakeCamera.TryAddShake(Constants.JACK_IN_SHAKE);

        //Event saying that the jack has been placed somewhere & checks if there are listeners
        if (onJackPlaced != null)
        {
            JackData data = new JackData() { PlacedJackID = jackID, SnappedSwitch = closestSwitch, IsOriginalPosition = closestSwitch.transform.position == jackSwitch.transform.position };
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

    public void configure(Switch js, int jackid, Switchboard board /*CharacterInfo character*/, int color)
    {
        jackSwitch = js;
        transform.position = jackSwitch.transform.position;
        initialPosition = transform.position;
        jackID = jackid;
        switchboard = board;

        //this._associatedCharacter = character;
        switch (color)
        {
            case 0:
                baseSprite = blueBaseSprite;
                dragSprite = blueDragSprite;
                placedSprite = bluePlacedSprite;
                break;
            case 1:
                baseSprite = greenBaseSprite;
                dragSprite = greenDragSprite;
                placedSprite = greenPlacedSprite;
                break;
            default:
                baseSprite = redBaseSprite;
                dragSprite = redDragSprite;
                placedSprite = redPlacedSprite;
                break;
        }
        _baseSpriteRenderer.sprite = baseSprite;
        _dragSpriteRenderer.sprite = dragSprite;
        _placedSpriteRenderer.sprite = placedSprite;
    }
}
