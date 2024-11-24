using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Knocker : MonoBehaviour
{
    private UnityEngine.Vector3 rotationPivot;
    private int speed = 50;
    // Start is called before the first frame update
    void Start()
    {
        DayManager.onStrike += knock;
        this.rotationPivot = new UnityEngine.Vector3(-4.566f, -2.382f, 0f);
    }

    //Knocks the bell, takes a speed.
    public void knock(int strikesLeft, bool recharge)
    {
        if (recharge)
            return;

        transform.GetComponent<AudioSource>().Play();
        StartCoroutine(knockLogic(speed));
        /* PSEUDOCODE
        ringBellSound()
        */
    }

    private IEnumerator knockLogic(int speed)
    {
        yield return knockForward(true, speed);
        ScreenShakeCamera.TryAddShake(Constants.KNOCKER_SHAKE);
        yield return transform.parent.GetComponentInChildren<Bell>().wiggle();
        yield return knockForward(false, speed);
    }

    private IEnumerator knockForward(bool fOrB, int speed)
    {
        float rotatedAngle = 0;

        while (Mathf.Abs(rotatedAngle) < Mathf.Abs(5))
        {
            float angleStep = speed * Time.deltaTime;

            if (rotatedAngle + angleStep > 5)
            {
                angleStep = 5 - rotatedAngle;
            }

            transform.RotateAround(this.rotationPivot, fOrB ? UnityEngine.Vector3.back : UnityEngine.Vector3.forward, angleStep);
            rotatedAngle += angleStep;

            yield return null;
        }
    }


}
