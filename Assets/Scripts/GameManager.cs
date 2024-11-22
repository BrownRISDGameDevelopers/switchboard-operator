using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TagsManager
{

    private HashSet<Tag> tagsSet;

    public delegate void OnTagAdded(Tag tag, TagsManager manager);
    public event OnTagAdded onAddTag;

    public TagsManager()
    {
        tagsSet = new HashSet<Tag>();
    }

    public void AddTags(Tag[] tags)
    {
        if (tagsSet == null)
            return;

        if (tags == null)
            return;

        foreach (Tag tag in tags)
        {
            if (tag == null)
                continue;

            onAddTag?.Invoke(tag, this);
            tagsSet.Add(tag);
        }
    }

    public bool HasAllTags(Tag[] tags)
    {
        bool hasAllTags = true;
        foreach (Tag tag in tags)
        {
            if (!HasTag(tag))
            {
                hasAllTags = false;
                break;
            }
        }
        return hasAllTags;
    }

    public bool HasNoTags(Tag[] tags)
    {
        bool hasDisallowedTags = false;
        foreach (Tag tag in tags)
        {
            if (HasTag(tag))
            {
                hasDisallowedTags = true;
                break;
            }
        }
        return hasDisallowedTags;
    }

    public bool HasTag(Tag tag)
    {
        return tagsSet.Contains(tag);
    }
}

// Overarching game manager
// not globally accessible, but only one should exist at a time 
// handles day changes, moving between scenes, stores tags, etc. 
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Day[] days;

    [SerializeField]
    private Ending[] endings;

    private DayManager dayManager;
    private LocationManager locationManager;
    private TagsManager tags;
    private int callsMissed = 0;
    private int callsRouted = 0;
    private int callsMessedUp = 0;
    private int money = 0;

    private static GameManager _existingGameManager = null;
    private static int currentDay = 0;

    void Awake()
    {
        /*if (_existingGameManager != null && _existingGameManager != this)
        {
            Debug.LogError("Critical error in GameManager, multiple game managers");
            Destroy(this);
            return;
        }
        if (!TryGetComponent<LocationManager>(out locationManager))
        {
            Debug.LogError("Critical error in GameManager, no location manager found in GameObject");
            return;
        }*/
        TryGetComponent<LocationManager>(out locationManager);
        _existingGameManager = this;


        tags = new TagsManager();

        tags.onAddTag += OnTagAddedCheckEnding;

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        //AsyncOperation load_op = SceneManager.LoadSceneAsync((int)Constants.SceneIndexTable.Game);
        //load_op.completed += (_) => LoadNewDay(days[currentDay]);
        LoadNewDay(days[currentDay]);
    }

    public void LoadNewDay(Day day)
    {
        dayManager = FindAnyObjectByType<DayManager>();
        if (dayManager != null)
        {
            dayManager.SetTagsReference(tags);
            dayManager.SetLocationReference(locationManager);
        }
    }


    private void OnTagAddedCheckEnding(Tag tag, TagsManager manager)
    {
        foreach (Ending end in endings)
        {
            if (end.endingType != EndingType.InstantTagsAcquired)
                continue;

            print("found instant tags acquired thing");
            bool hasTags = tags.HasAllTags(end.requiredTags);
            if (hasTags)
            {
                print("tags");
                // GO TO THIS ENDING
                EnterEnding(end);
            }
        }
    }

    public void EndCurrentDay()
    {
        // TODO: Potential for cutscenes and/or checks for early endings 
        //

        currentDay++;
        if (currentDay >= days.Length)
        {
            // End the game
            ReturnToMenu();
            return;
        }

        // Logic for cutscenes + early endings

        SceneManager.LoadScene((int)Constants.SceneIndexTable.EndOfDay);
    }

    public void EnterEndOfDay()
    {
    }

    private void EnterEnding(Ending end)
    {
        print("ENTERING ENDING");
        SceneManager.LoadScene((int)Constants.SceneIndexTable.Ending);
        Ending_Displayer disp = FindFirstObjectByType<Ending_Displayer>();
        disp.displayEnding(end);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene((int)Constants.SceneIndexTable.Menu);
    }

    public void PayPlayer(int amount)
    {
        money += amount;
    }
}
