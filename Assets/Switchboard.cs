using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;

public class Switchboard : MonoBehaviour
{
    public int rows = 3;
    public int columns = 3;
    public float xSpacing = 0.75f;
    public float ySpacing = 1f;
    public UnityEngine.Vector3[,] outlets;
    public UnityEngine.Vector3 centerOfGrid = new UnityEngine.Vector3(0f, 3f, 0f);
    public Jack jack1;
    public Jack jack2;
    public Jack jack3;

    // Start is called before the first frame update
    void Start()
    {
        //Define positions of outlets
        outlets = new UnityEngine.Vector3[rows,columns];
        for(int i=0; i<rows; i++){
            for(int j=0; j<columns; j++){
                outlets[i,j] = centerOfGrid + new UnityEngine.Vector3(i*xSpacing - xSpacing*(rows-1)/2, j*ySpacing - ySpacing*(columns-1)/2, 0);
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = outlets[i,j];
                cube.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
            }
        }

        //Spawn in jacks and instantiate them with necessary values
        jack1 = Instantiate(jack1, new UnityEngine.Vector3(0, 0, 0), UnityEngine.Quaternion.identity);
        jack1.configure(new UnityEngine.Vector3(-1f,0f,0f), 0, this);
        jack2 = Instantiate(jack2, new UnityEngine.Vector3(-1, 0, 0), UnityEngine.Quaternion.identity);
        jack2.configure(new UnityEngine.Vector3(0f,0f,0f), 1, this);
        jack3 = Instantiate(jack3, new UnityEngine.Vector3(1, 0, 0), UnityEngine.Quaternion.identity);
        jack3.configure(new UnityEngine.Vector3(1f,0f,0f), 2, this);

        
    }

    //Checks the position of all outlets relative to the passed position of the mouse/jack, returns the nearest position of outlet
    public UnityEngine.Vector3 GetClosestOutlet(UnityEngine.Vector3 position, UnityEngine.Vector3 originalPosition)
    {
        float shortestDistance = UnityEngine.Vector3.Distance(position, originalPosition);
        UnityEngine.Vector3 closestOutlet = originalPosition;
        foreach(UnityEngine.Vector3 outlet in outlets){
            float distance = UnityEngine.Vector3.Distance(position, outlet);
            if (distance < shortestDistance){
                shortestDistance = distance;
                closestOutlet = outlet;
            }
        }
        return closestOutlet;
    }
}
