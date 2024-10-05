using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required when using Event data.

public class Book : MonoBehaviour
{
    [SerializeField] float deselectedXPosition = 13.56F;
    [SerializeField] float selectedXPosition = 0.0F;
    bool focused = false;
	
    void Start()
    {
        transform.position = new Vector3(deselectedXPosition, transform.position.y, transform.position.z);
    }

    void Update()
    {
        updateFocusState();
        updatePosition();
    }

    // Manages clicking on and off of book in order to bring it on and offscreen
    Ray ray;
    void updateFocusState()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (focused)
        {
            if (Input.GetMouseButtonDown(0) && !Physics.Raycast(ray, out RaycastHit hit))
            {
                focused = false;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out RaycastHit hit))
            {
                focused = (hit.collider.gameObject.tag == "Book");
            }
        }
    }

    // Updates book position based on focus state
    void updatePosition()
    {
        if (focused)
        {
            transform.position = new Vector3(selectedXPosition, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(deselectedXPosition, transform.position.y, transform.position.z);
        }
    }
}