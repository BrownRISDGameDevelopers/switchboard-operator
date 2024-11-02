using System;
using UnityEditor;
using UnityEngine;

public class Switchboard : MonoBehaviour
{
    public GameObject switchPrefab;
    public GameObject jackPrefab;
    public int rows = 5;
    public int columns = 6;
    public int jackCount = 6;
    private float xSpacing = 0.89f;
    private float ySpacing = 0.89f;
    private float initialSwitchX = -1.957f;
    private float initialSwitchY = 1.949f;

    public UnityEngine.Vector3 centerOfJackRow = new UnityEngine.Vector3(0, -4, 0);
    private UnityEngine.Vector3[,] switchPositions;
    private Switch[,] switches;
    private Jack[] jacks;
    private Switch[] jackSwitches;

    void Awake()
    {

        switches = new Switch[columns, rows];
        jackSwitches = new Switch[jackCount];
        jacks = new Jack[jackCount];

    }

    void Start()
    {
        initialSwitchX += transform.position.x;
        initialSwitchY += transform.position.y;
        ProduceSwitches();
        ProduceJacks();
    }

    public void ProduceJacks()
    {
        for (int i = 0; i < jackCount; i++)
        {
            UnityEngine.Vector3 position = centerOfJackRow + new UnityEngine.Vector3(
                i * xSpacing - xSpacing * (jackCount - 1) / 2, 0, 0);
            GameObject go_switch = Instantiate(switchPrefab, position, UnityEngine.Quaternion.identity);
            GameObject go_jack = Instantiate(jackPrefab, position, UnityEngine.Quaternion.identity);

            jackSwitches[i] = go_switch.GetComponent<Switch>();
            jacks[i] = go_jack.GetComponent<Jack>();

            jackSwitches[i].locationData = new Location()
            {
                Valid = true,
                Index = 0,
                Letter = 'j',
                Number = i
            };
            jacks[i].configure(jackSwitches[i], i, this);
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
                if (i == 2)
                {
                    a += 0.769f;
                }
                else if (i == 4)
                {
                    a += 0.83f;
                }
                GameObject t_switch = Instantiate(
                    switchPrefab,
                    new UnityEngine.Vector3(
                        initialSwitchX + i * xSpacing + a,
                        initialSwitchY - j * ySpacing,
                        0
                    ),
                    UnityEngine.Quaternion.identity);
                t_switch.GetComponent<Switch>().locationData = new Location()
                {
                    Valid = true,
                    Index = 0, //Index not used at all (until further notice)
                    Letter = (char)(65 + j),
                    Number = 1 + i
                };
                switches[i, j] = t_switch.GetComponent<Switch>();
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

    //Checks the position of all switches relative to the passed position of the mouse/jack, returns the nearest position of switch
    public Switch GetClosestSwitchPosition(Jack jack)
    {
        float shortestDistance = UnityEngine.Vector3.Distance(jack.transform.position, jack.jackSwitch.transform.position);
        Switch closestSwitch = null;

        foreach (Switch c_switch in switches)
        {
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
