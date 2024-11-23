using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void SetDayManager(DayManager _dayManager){
        dayManager = _dayManager;
    }

    void Update()
    {
        talkingTo = dayManager.GetCurrentlyInDialogue();
        setSong(talkingTo);     
    }

    void setSong(CharacterInfo _talkingTo){
        
        int targetIndex = -1;

        for (int i = 0; i < charNames.Length; i++){
            if (charNames[i] == talkingTo){
                targetIndex = i;
            }
        }

        if (targetIndex < 0)
        {
            soloCurrentSong(NPCMusic);
            return;
        }
        else
        {
            soloCurrentSong(CharAudioSources[targetIndex]);
        }

    }

    void soloCurrentSong(AudioSource currentSong){
    //    m_RectTransform.anchoredPosition = Vector2.Lerp(currentPosition, finalPosition, 5F * Time.deltaTime);

        currentSong.volume = 1;

        foreach (AudioSource aud in CharAudioSources){
            if (aud != currentSong){
                aud.volume = 0;
            }
        }

        if (NPCMusic != currentSong){
            NPCMusic.volume = 0;
        }

    }
}