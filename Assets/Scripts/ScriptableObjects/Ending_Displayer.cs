using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Ending_Displayer : MonoBehaviour
{
    [SerializeField]
    private Ending displayEndingTest;

    private string endingText;
    private Sprite[] endingPortraits;
    private float _secsPerFrame = 1.0f;
    private float _curSecsPerFrame = 1.0f;
    private int _currentAnimFrame = 0;

    // Reference the UI elements in the Inspector
    [SerializeField] private TMP_Text endingTextUI; // Assign via the Inspector or code
    [SerializeField] private Image endingPortraitUI; // Assign via the Inspector or code

    // Start is called before the first frame update
    void Start()
    {
        //Ensure that UI elements are cleared initially
        if (endingTextUI != null) endingTextUI.text = "";
        if (endingPortraitUI != null) endingPortraitUI.sprite = null;


        if (displayEndingTest)
            displayEnding(displayEndingTest);
    }

    // Update is called once per frame
    void Update()
    {
        _curSecsPerFrame += Time.deltaTime;
        if (_curSecsPerFrame > _secsPerFrame)
        {
            _curSecsPerFrame -= _secsPerFrame;
            _currentAnimFrame++;
            if (endingPortraits.Length <= _currentAnimFrame)
                return;

            endingPortraitUI.sprite = endingPortraits[_currentAnimFrame];
        }
    }

    //Call this when you have an ending you want to show. Needs something to prevent controls. 
    public void displayEnding(Ending myEnding)
    {
        endingText = myEnding.text;
        endingPortraits = myEnding.anim;
        // Update UI components
        if (endingTextUI != null)
        {
            endingTextUI.text = endingText;
        }

        if (endingPortraitUI != null && endingPortraits.Length > 0)
        {
            // For now, I'm going to display the first portrait (you can modify this to cycle through all portraits)
            endingPortraitUI.sprite = endingPortraits[0];
            endingTextUI.text = endingText;
        }

        if (endingPortraits.Length <= 0)
        {
            //you fucked up if this happened. 
        }
    }

}
