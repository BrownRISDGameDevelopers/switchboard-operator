using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockInButton : MonoBehaviour
{
    public int jackSet = 0;

    public delegate void OnJackLock(int jackSetId);
    public static event OnJackLock onJackLock;

    public SpriteRenderer unpressed;
    public SpriteRenderer pressed;

    [SerializeField] AudioSource SFX_button_pressed;

    void Start()
    {
        unpressed.color = Color.white;
        pressed.color = Color.clear;
    }

    //When the mouse is clicked on the collider
    void OnMouseDown()
    {
        // Debug.Log("On mouse down");
        SFX_button_pressed.Play();
        onJackLock(jackSet);
        unpressed.color = Color.clear;
        pressed.color = Color.white;
        print("Bum");
        StartCoroutine(undoClick());
    }

    IEnumerator undoClick()
    {
        yield return new WaitForSeconds(0.2f);
        unpressed.color = Color.white;
        pressed.color = Color.clear;
    }

    //Gets the current mouse position as a Vector3
    UnityEngine.Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(new UnityEngine.Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
    }

}
