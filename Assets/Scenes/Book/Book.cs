using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq; // Required when using Event data.

public class Book : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] float deselectedXPosition;
    [SerializeField] float selectedXPosition;
    [SerializeField] float YPosition;
    [SerializeField] GameObject leftPage;
    [SerializeField] GameObject rightPage;
    bool focused = false;
    RectTransform m_RectTransform;
    LocationManager locationManager;

    List<Nametag> nametagList = new List<Nametag>();
    List<CharacterInfo> characterList = new List<CharacterInfo>();
    List<Location> locationList = new List<Location>();

    void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();

        m_RectTransform.anchoredPosition = new Vector2(deselectedXPosition, YPosition);

        locationManager = FindObjectOfType<LocationManager>();

        characterList = locationManager.GetCharacterList();
        locationList = locationManager.GetLocationList();

        // Initialize nametags in each page
        addNametagsToList(leftPage);
        addNametagsToList(rightPage);

        updateNamesInBook();
    }

    void Update()
    {
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

    // Adds nametag children to nametagList by parent page
    void addNametagsToList(GameObject page)
    {
        foreach (Transform child in page.transform)
        {
            Nametag nametag = child.GetComponent<Nametag>();
            if (nametag == null)
            {
                continue;
            }

            nametagList.Add(nametag);
        }
    }

    // Updates book position based on focus state
    void updatePosition()
    {

        Vector2 currentPosition = m_RectTransform.anchoredPosition;
        Vector2 finalPosition;

        if (focused)
        {
            finalPosition = new Vector2(selectedXPosition, YPosition);
        }
        else
        {
            finalPosition = new Vector2(deselectedXPosition, YPosition);
        }

        m_RectTransform.anchoredPosition = Vector2.Lerp(currentPosition, finalPosition, 5F * Time.deltaTime);

        // print(currentPosition);
        // print(finalPosition);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (focused)
        {
            focused = false;
        }
        else
        {
            focused = true;
        } 
    }
}
