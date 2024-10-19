using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq; // Required when using Event data.

public class Book : MonoBehaviour
{
    [SerializeField] float deselectedXPosition;
    [SerializeField] float selectedXPosition;
    [SerializeField] GameObject leftPage;
    [SerializeField] GameObject rightPage;
    bool focused = false;
    RectTransform m_RectTransform;
    LocationManager locationManager;

    List<Nametag> nametagList = new List<Nametag>();
    List<CharacterInfo> characterList = new List<CharacterInfo>();
	
    void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();

        m_RectTransform.anchoredPosition = new Vector2(deselectedXPosition, 0.0F);
        
        locationManager = FindObjectOfType<LocationManager>();

        characterList = locationManager.GetCharacterList();

        // Initialize nametags in each page
        addNametagsToList(leftPage);
        addNametagsToList(rightPage);

        updateNamesInBook();
    }

    void Update()
    {
        updateFocusState();
        updatePosition();
    }

    void updateNamesInBook()
    {
        for (int i = 0; i < characterList.Count(); i++) 
        {
            Nametag currentTag = nametagList[i];
            CharacterInfo currentChar = characterList[i];
            Location currentLoc = locationManager.GetLocationFromCharacter(currentChar);

            currentTag.nameText.text = currentChar.CharName;
            currentTag.locationText.text = currentLoc.Number + "" + currentLoc.Letter;
        }
    }

    void addNametagsToList(GameObject page)
    {
        foreach (Transform child in page.transform)
        {
            Nametag nametag = child.GetComponent<Nametag>();
            if (nametag == null)
                continue; 

            nametagList.Add(nametag);
        }
    }

    // Manages clicking on and off of book in order to bring it on and offscreen
    Ray ray;
    void updateFocusState()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (focused)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                focused = false;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
            {
                focused = true;
            }
        }
    }

    // Updates book position based on focus state
    void updatePosition()
    {

        Vector2 currentPosition = m_RectTransform.anchoredPosition;
        Vector2 finalPosition;

        if (focused)
        {
            finalPosition = new Vector2(selectedXPosition, 0.0F);
        }
        else
        {
            finalPosition = new Vector2(deselectedXPosition, 0.0F);
        }

        m_RectTransform.anchoredPosition = Vector2.Lerp(currentPosition, finalPosition, 5F * Time.deltaTime);

        // print(currentPosition);
        // print(finalPosition);
    }
}