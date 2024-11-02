using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public Location locationData;
    public Sprite blinkOnSprite;
    public Sprite blinkOffSprite;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponentInChildren<SpriteRenderer>().sprite = blinkOffSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void blinkSwitch(int time){
        int multiplier = 10/(time+1);
        if(math.cos(time*multiplier) > 0){
            this.GetComponentInChildren<SpriteRenderer>().sprite = blinkOnSprite;
        }else{
            this.GetComponentInChildren<SpriteRenderer>().sprite = blinkOffSprite;
        }
    }
    //Setter just in case
    public void setSpriteOff(){
        this.GetComponentInChildren<SpriteRenderer>().sprite = blinkOffSprite;
    }
    //Setter just in case
    public void setSpriteOn(){
        this.GetComponentInChildren<SpriteRenderer>().sprite = blinkOnSprite;
    }
}
