using UnityEngine;

public class Switchboard : MonoBehaviour
{
    public Switch switchPrefab;
    public Jack jackPrefab;
    public int rows = 3;
    public int columns = 3;
    public int jackCount = 5;
    public float xSpacing = 0.75f;
    public float ySpacing = 1f;
    public UnityEngine.Vector3 centerOfGrid = new UnityEngine.Vector3(0, 3, 0);
    public UnityEngine.Vector3 centerOfJackRow = new UnityEngine.Vector3(0, 0, 0);
    private UnityEngine.Vector3[,] switchPositions;
    private Switch[,] switches;
    private Jack[] jacks;
    private Switch[] jackSwitches;

    void Start()
    {
        //Define positions of outlets
        switchPositions = new UnityEngine.Vector3[rows, columns];
        switches = new Switch[rows, columns];
        jackSwitches = new Switch[3];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                switchPositions[i, j] = centerOfGrid + new UnityEngine.Vector3(i * xSpacing - xSpacing * (rows - 1) / 2, j * ySpacing - ySpacing * (columns - 1) / 2, 0);
                // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // cube.transform.position = outlets[i,j];
                // cube.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
                Switch st = Instantiate(switchPrefab, switchPositions[i, j], UnityEngine.Quaternion.identity);
                st.locationData = new Location() { Valid = true, Index = 0, Letter = (char)(65 + (columns - j - 1)), Number = i + 1 };
                switches[i, j] = st;
            }
        }

        ProduceSwitches();
        ProduceJacks();
        /*
        //Spawn in jacks and instantiate them with necessary values
        originalJackPositions = new UnityEngine.Vector3[3];
        originalJackPositions[0] = new UnityEngine.Vector3(0, 0, 0);
        originalJackPositions[1] = new UnityEngine.Vector3(-1, 0, 0);
        originalJackPositions[2] = new UnityEngine.Vector3(1, 0, 0);

        //Instantiate switches for jacks
        Switch s = Instantiate(switchPrefab, originalJackPositions[0], UnityEngine.Quaternion.identity);
        s.locationData = new Location() { Valid = true, Index = 0, Letter = 'j', Number = 0 };
        jackSwitches[0] = s;
        s = Instantiate(switchPrefab, originalJackPositions[1], UnityEngine.Quaternion.identity);
        s.locationData = new Location() { Valid = true, Index = 0, Letter = 'j', Number = 1 };
        jackSwitches[1] = s;
        s = Instantiate(switchPrefab, originalJackPositions[2], UnityEngine.Quaternion.identity);
        s.locationData = new Location() { Valid = true, Index = 0, Letter = 'j', Number = 2 };
        jackSwitches[2] = s;

        jack1 = Instantiate(jack1, originalJackPositions[0], UnityEngine.Quaternion.identity);
        jack1.configure(jackSwitches[0], 0, this);
        jack2 = Instantiate(jack2, originalJackPositions[1], UnityEngine.Quaternion.identity);
        jack2.configure(jackSwitches[1], 1, this);
        jack3 = Instantiate(jack3, originalJackPositions[2], UnityEngine.Quaternion.identity);
        jack3.configure(jackSwitches[2], 2, this);
        */
    }

    public void ProduceJacks()
    {
        jackSwitches = new Switch[jackCount];
        jacks = new Jack[jackCount];

        for (int i = 0; i < jackCount; i++)
        {
            UnityEngine.Vector3 position = centerOfJackRow + new UnityEngine.Vector3(
                i * xSpacing - xSpacing * (jackCount - 1) / 2,
                0,
                0);
            Switch go_switch = Instantiate(switchPrefab, position, UnityEngine.Quaternion.identity);
            Jack go_jack = Instantiate(jackPrefab, position, UnityEngine.Quaternion.identity);

            jackSwitches[i] = go_switch;
            jacks[i] = go_jack;

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
        switchPositions = new UnityEngine.Vector3[columns, rows];
        switches = new Switch[columns, rows];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                switchPositions[i, j] =
                    centerOfGrid +
                    new UnityEngine.Vector3(
                        i * xSpacing - xSpacing * (columns - 1) / 2,
                        j * ySpacing - ySpacing * (rows - 1) / 2,
                        0);

                Switch go_switch = Instantiate(switchPrefab, switchPositions[i, j], UnityEngine.Quaternion.identity);
                Switch c_switch = go_switch.GetComponent<Switch>();
                go_switch.GetComponent<Switch>().locationData = new Location()
                {
                    Valid = true,
                    Index = 0,
                    Letter = (char)(j + 65),
                    Number = i + 1
                };

                switches[i, j] = go_switch.GetComponent<Switch>();
            }
        }
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
}
