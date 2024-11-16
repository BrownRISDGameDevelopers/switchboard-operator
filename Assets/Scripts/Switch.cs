using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public Location locationData;
    public Sprite blinkOnSprite;
    public Sprite blinkOffSprite;
    public bool isTaken;

    private SpriteRenderer _spriteRenderComponent;


    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderComponent = this.GetComponentInChildren<SpriteRenderer>();

        if (_spriteRenderComponent == null)
        {
            Debug.LogError("No sprite render component in switch");
            return;
        }
        _spriteRenderComponent.sprite = blinkOffSprite;
    }

    public void blinkSwitch(float secondsLeft)
    {
        if (_spriteRenderComponent == null)
            return;

        if (Mathf.Cos((secondsLeft >= 0) ? 2000 / (secondsLeft + 10) : 25 * secondsLeft) > 0)
            _spriteRenderComponent.sprite = blinkOnSprite;
        else
            _spriteRenderComponent.sprite = blinkOffSprite;
    }
    //Setter just in case
    public void setSpriteOff()
    {
        if (_spriteRenderComponent == null)
            return;
        _spriteRenderComponent.sprite = blinkOffSprite;
    }

    //Setter just in case
    public void setSpriteOn()
    {
        if (_spriteRenderComponent == null)
            return;
        _spriteRenderComponent.sprite = blinkOnSprite;
    }
}
