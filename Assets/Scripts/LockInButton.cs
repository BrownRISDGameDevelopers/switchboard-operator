using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockInButton : MonoBehaviour
{
    public int jackSet = 0;

    public delegate void OnJackLock(int jackSetId);
    public static event OnJackLock onJackLock;

    [SerializeField] AudioSource SFX_button_pressed;

    //When the mouse is clicked on the collider
    void OnMouseDown()
    {
        // Debug.Log("On mouse down");
        SFX_button_pressed.Play();
        onJackLock(jackSet);
    }

    //Gets the current mouse position as a Vector3
    UnityEngine.Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(new UnityEngine.Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
    }

}
