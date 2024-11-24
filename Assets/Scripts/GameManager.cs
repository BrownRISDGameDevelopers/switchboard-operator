using System.Collections;
using System.Collections.Generic;
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
    [SerializeField]
    private Ending defaultEnding;

    private DayManager dayManager;
    private LocationManager locationManager;
    private TagsManager tags;

    private static GameManager _existingGameManager = null;

    private static int currentDay = 0;

    private const int DAY_OFFSET = 2;

    public MusicManager musicManager;

    private Ending _toDisplayEnding = null;

    void Awake()
    {
        if (_existingGameManager != null && _existingGameManager != this)
        {
            Debug.LogError("Critical error in GameManager, multiple game managers");
            Destroy(this);
            return;
        }
        if (!TryGetComponent<LocationManager>(out locationManager))
        {
            Debug.LogError("Critical error in GameManager, no location manager found in GameObject");
            return;
        }
        TryGetComponent<LocationManager>(out locationManager);
        _existingGameManager = this;


        tags = new TagsManager();

        tags.onAddTag += OnTagAddedCheckEnding;
        DayManager.OnDayEnd += OnDayEnd;
        DayManager.onStrike += OnStrike;

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        // AsyncOperation load_op = SceneManager.LoadSceneAsync((int)Constants.SceneIndexTable.Game);
        // load_op.completed += (_) => LoadNewDay(days[currentDay]);
        LoadNewDay(days[currentDay]);
    }

    private void OnDayEnd()
    {
        // Check for endings
        // Go to transition
        // prepare to go to next day
        //
        // TODO: Potential for cutscenes and/or checks for early endings 
        //

        if (CheckEndingsForType(EndingType.EndOfAnyDay) != null)
            return;
        currentDay++;
        if (currentDay >= days.Length)
        {
            // End the game
            if (CheckEndingsForType(EndingType.EndOfDays) != null)
                return;   //ReturnToMenu();
            EnterEnding(defaultEnding);
            return;
        }

        // Logic for cutscenes + early endings

        SceneManager.LoadScene((int)Constants.SceneIndexTable.EndOfDay);

    }

    private void OnStrike(int strikes, bool recharge)
    {

        if (strikes < 3)
            return;

        // Failure, go to failure

    }

    IEnumerator WaitToGoToGameOver()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene((int)Constants.SceneIndexTable.Ending);


    }

    public void LoadNewDay(Day day)
    {
        SceneManager.LoadScene(currentDay + DAY_OFFSET);
        SceneManager.sceneLoaded += OnDayLoadFinish;
    }

    void OnDayLoadFinish(Scene scene, LoadSceneMode mode)
    {

        dayManager = FindObjectOfType<DayManager>();
        if (dayManager != null)
        {
            dayManager.SetupDayManager(tags, locationManager, currentDay < days.Length ? days[currentDay] : null);

            // TODO: Set day for day manager and make sure that works
            // Make sure music manager has access to updated day
            musicManager.SetDayManager(dayManager);
            // print("musicmanager set");

        }
        SceneManager.sceneLoaded -= OnDayLoadFinish;
    }

    private Ending CheckEndingsForType(EndingType type)
    {
        foreach (Ending end in endings)
        {
            if (end.endingType != type)
                continue;

            bool hasTags = tags.HasAllTags(end.requiredTags);
            if (hasTags)
            {
                // GO TO THIS ENDING
                EnterEnding(end);
                return end;
            }
        }
        return null;
    }

    private void OnTagAddedCheckEnding(Tag tag, TagsManager manager)
    {
        CheckEndingsForType(EndingType.InstantTagsAcquired);
    }

    private void EnterEnding(Ending end)
    {
        print("ENTERING ENDING");
        SceneManager.LoadScene((int)Constants.SceneIndexTable.Ending);
        _toDisplayEnding = end;
        SceneManager.sceneLoaded += OnEnterEnding;
    }

    private void OnEnterEnding(Scene scene, LoadSceneMode mode)
    {
        Ending_Displayer disp = FindFirstObjectByType<Ending_Displayer>();
        disp?.displayEnding(_toDisplayEnding);
        SceneManager.sceneLoaded -= OnEnterEnding;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene((int)Constants.SceneIndexTable.Menu);
    }
}
