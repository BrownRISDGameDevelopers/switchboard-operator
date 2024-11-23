using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class MusicManager : MonoBehaviour

{
    DayManager dayManager;
    CharacterInfo talkingTo;
    public AudioSource bassline;
    public AudioSource NPCMusic;
    public AudioSource[] CharAudioSources;
    public CharacterInfo[] charNames;



    void Start()
    {
        dayManager = FindObjectOfType<DayManager>();
    }

    void Update()
    {
       talkingTo = dayManager.GetCurrentlyInDialogue();
       print(talkingTo.CharName);
    }
}