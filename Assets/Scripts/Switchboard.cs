using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Switchboard : MonoBehaviour
{
    public GameObject switchPrefab;
    public GameObject jackPrefab;
    public GameObject lockInPrefab;
    public int rows = 5;
    public int columns = 6;
    public int jackCount = 6;
    private float xSpacing = 0.89f;
    private float ySpacing = 0.89f;
    private float initialSwitchX = 0.75f;//-1.957f;
    private float initialSwitchY = 2.25f;//1.949f;


    public Transform[] columnLocations;

    public UnityEngine.Vector3 centerOfJackRow = new UnityEngine.Vector3(0, -4, 0);
    private UnityEngine.Vector3[,] switchPositions;
    private Switch[,] switches;
    private Jack[] jacks;
    private Switch[] jackSwitches;
    private LockInButton[] lockInButtons;

    void Awake()
    {
        switches = new Switch[columns, rows];
        jackSwitches = new Switch[jackCount];
        jacks = new Jack[jackCount];
        lockInButtons = new LockInButton[jackCount / 2];
        //initialSwitchX += transform.position.x;
        //initialSwitchY += transform.position.y;
        ProduceSwitches();
        ProduceJacks();
    }


    private Transform GetColLocationFromIndex(int i)
    {
        Transform ret = transform;
        int col = i / 2;
        if (columnLocations.Length > col)
        {
            ret = columnLocations[col];
        }
        return ret;
    }

    public void ProduceJacks()
    {
        for (int i = 0; i < jackCount; i++)
        {
            Transform initTrans = GetColLocationFromIndex(i);
            Vector3 pos = new Vector3(initTrans.position.x + (i % 2 * xSpacing * transform.localScale.x), initTrans.position.y - ((rows) * ySpacing * transform.localScale.y), 0);

            //UnityEngine.Vector3 position = centerOfJackRow + new UnityEngine.Vector3(
            //transform.localScale.x * (i * xSpacing - xSpacing * (jackCount - 1) / 2), 3.0f, 0);
            GameObject go_switch = Instantiate(switchPrefab, pos, UnityEngine.Quaternion.identity);
            GameObject go_jack = Instantiate(jackPrefab, pos, UnityEngine.Quaternion.identity);

            jackSwitches[i] = go_switch.GetComponent<Switch>();
            jackSwitches[i].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            
            jacks[i] = go_jack.GetComponentInChildren<Jack>();

            jackSwitches[i].locationData = new Location()
            {
                Valid = true,
                Letter = 'j',
                Number = i
            };
            jackSwitches[i].isTaken = true;
            jacks[i].configure(jackSwitches[i], i, this, (int)math.floor(i / 2));
        }

        int lockInNumber = jackCount / 2;
        for (int i = 0; i < lockInNumber; i++)
        {
            Transform initTrans = GetColLocationFromIndex(i * 2);
            Vector3 pos = new Vector3(initTrans.position.x + ((xSpacing / 2) * transform.localScale.x), initTrans.position.y - ((rows + 1) * ySpacing * transform.localScale.y), 0);

            GameObject button = Instantiate(lockInPrefab, pos, UnityEngine.Quaternion.identity);
            lockInButtons[i] = button.GetComponent<LockInButton>();
            lockInButtons[i].jackSet = i;
        }
    }

    public void ProduceSwitches()
    {
        //Define positions of outlets
        // switchPositions = new UnityEngine.Vector3[columns, rows];
        for (int j = 0; j < rows; j++)
        {
            float a = 0f; //Additional increment for stepping
            for (int i = 0; i < columns; i++)
            {
                Transform initTrans = GetColLocationFromIndex(i);
                Vector3 pos = new Vector3(initTrans.position.x + (i % 2 * xSpacing * transform.localScale.x), initTrans.position.y - (j * ySpacing * transform.localScale.y), 0);

                GameObject t_switch = Instantiate(
                    switchPrefab,
                    pos,
                    /*new UnityEngine.Vector3(
                         transform.localScale.x * (initialSwitchX + (i * xSpacing + a) - (xSpacing * (columns / 2))),
                         transform.localScale.y * (initialSwitchY + (-j * ySpacing) + (ySpacing * (rows / 2))),
                        0
                    ),*/
                    UnityEngine.Quaternion.identity
                    );

                Switch comp = t_switch.GetComponent<Switch>();
                comp.locationData = new Location()
                {
                    Valid = true,
                    //Index = (j * columns) + i,
                    Letter = (char)(65 + j),
                    Number = 1 + i
                };
                comp.isTaken = false;
                switches[i, j] = comp;
            }
        }

        // for (int i = 0; i < columns; i++)
        // {
        //     for (int j = 0; j < rows; j++)
        //     {
        //         switchPositions[i, j] =
        //             centerOfGrid +
        //             new UnityEngine.Vector3(
        //                 i * xSpacing - xSpacing * (columns - 1) / 2,
        //                 j * ySpacing - ySpacing * (rows - 1) / 2,
        //                 0);

        //         Switch go_switch = Instantiate(switchPrefab, switchPositions[i, j], UnityEngine.Quaternion.identity);
        //         go_switch.GetComponent<Switch>().locationData = new Location()
        //         {
        //             Valid = true,
        //             Index = 0,
        //             Letter = (char)(j + 65),
        //             Number = i + 1
        //         };

        //         switches[i, j] = go_switch.GetComponent<Switch>();
        //     }
        // }
    }

    public void SetSwitchTiming(Location loc, float time)
    {
        // Debug.Log("pos: " + loc.Letter.ToString() + loc.Number.ToString() + " ind: " + loc.GetIndex(columns).ToString());
        Switch _switch = switches[loc.GetIndex(columns) % columns, loc.GetIndex(columns) / columns];
        _switch?.blinkSwitch(time);
    }

    //Checks the position of all switches relative to the passed position of the mouse/jack, returns the nearest position of switch
    public Switch GetClosestSwitchPosition(Jack jack)
    {
        float shortestDistance = math.INFINITY;
        Switch closestSwitch = null;

        foreach (Switch c_switch in switches)
        {

            // + Vector3.up * 0.6f, for position
            float distance = UnityEngine.Vector3.Distance(jack.transform.position, c_switch.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestSwitch = c_switch;
            }
        }
        if (closestSwitch == null)
        {
            closestSwitch = jackSwitches[jack.jackID];
        }
        return closestSwitch;
    }

    public Switch[,] GetSwitches()
    {
        return this.switches;
    }

    internal Jack[] GetJacks()
    {
        return this.jacks;
    }
}
