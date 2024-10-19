using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending_Displayer : MonoBehaviour
{
    private string endingText 
    private Texture2D[] endingPortraits

    // Reference the UI elements in the Inspector
    [SerializeField] private Text endingTextUI; // Assign via the Inspector or code
    [SerializeField] private RawImage endingPortraitUI; // Assign via the Inspector or code

    // Start is called before the first frame update
    void Start()
    {
        //Ensure that UI elements are cleared initially
        if (endingTextUI != null) endingTextUI.text = "If this shows we have a problem";
        if (endingPortraitUI != null) endingPortraitUI.texture = null;
    } 
    //Call this when you have an ending you want to show. Needs something to prevent controls. 
    public void displayEnding(Ending myEnding) {
        endingText = myEnding.text  
        endingPortraits = myEnding.portraits  
         // Update UI components
        if (endingTextUI != null)
        {
            endingTextUI.text = endingText;
        }

        if (endingPortraitUI != null && endingPortraits.Length > 0)
        {
            // For now, I'm going to display the first portrait (you can modify this to cycle through all portraits)
            endingPortraitUI.texture = endingPortraits[0];
            endingTextUI.text = endingText; 
        } 
        if (endingPortraits.Length <=0) {
            //you fucked up if this happened. 
        }
        // Optionally, disable player controls here
        // Something like DisablePlayerControls();
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
