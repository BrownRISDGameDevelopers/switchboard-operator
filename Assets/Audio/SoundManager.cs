using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Modified from https://www.daggerhartlab.com/unity-audio-and-sound-manager-singleton-script/
public class SoundManager : MonoBehaviour
{

   [Header("SoundEmitters pool")]
   [SerializeField] private AudioSource[] _pool = default;
   [SerializeField] private int _initialSize = 10;
   private Queue<AudioSource> availableSources = new Queue<AudioSource>();
   private HashSet<AudioSource> currentlyPlaying = new HashSet<AudioSource>();

   [SerializeField] private AudioCueSO[] loopingSounds;



   /*[Header("Audio control")]
   [SerializeField] private AudioMixer audioMixer = default;*/
   [Range(0f, 1f)]
   [SerializeField] private float _masterVolume = 1f;
   [Range(0f, 1f)]
   [SerializeField] private float _musicVolume = 1f;
   [Range(0f, 1f)]
   [SerializeField] private float _sfxVolume = 1f;


   // Audio players components.
   //public AudioSource[] EffectsSource;
   //public AudioSource MusicSource;

   // Singleton instance.
   public static SoundManager Audio = null;

   private GameObject soundSourceHolder;


   /*public AudioSource EmptyEffectsSource()
   {

	   for (int i = 0; i < EffectsSource.Length; i++)
	   {
		   if (!EffectsSource[i].isPlaying)
		   {
			   return EffectsSource[i];
		   }
	   }

	   return EffectsSource[0];
   }*/

   private void OnLevelWasLoaded(int level)
   {
	  if (level == 4)
	  {
		 foreach (AudioDataSO data in loopingSounds)
		 {
			Audio?.PlaySFXSound(data, Vector3.zero);
		 }
	  }
   }



   private void Awake()
   {

	  if (Audio is null)
	  {
		 Audio = this;
		 InitAudioSources();
		 //DontDestroyOnLoad(gameObject);

		 SceneManager.sceneLoaded += InitOnLoad;

	  }
	  else if (Audio != this)
	  {
		 Destroy(this);
	  }


   }

   public void InitOnLoad(Scene scene, LoadSceneMode mode)
   {
	  InitAudioSources();
   }

   private void OnDestroy()
   {
	  SceneManager.sceneLoaded -= InitOnLoad;

	  if (Audio == this)
	  {
		 Audio = null;
	  }
   }

   private void Start()
   {

   }


   private void Update()
   {
	  if (currentlyPlaying.Count > 0)
	  {
		 // THERE IS DEFINITELY A WAY TO MAKE THIS MORE SPACE EFFICIENT
		 AudioSource[] copyArray = new AudioSource[currentlyPlaying.Count];
		 currentlyPlaying.CopyTo(copyArray);
		 foreach (AudioSource source in copyArray)
		 {
			if (!source.isPlaying)
			{
			   // We don't remove from currently playing here because its in foreach loop, instead we remove from hashset when we get available sources
			   availableSources.Enqueue(source);
			   currentlyPlaying.Remove(source);
			   source.transform.parent = soundSourceHolder.transform;
			}
		 }
	  }
   }


   public void InitAudioSources()
   {

	  soundSourceHolder = new GameObject("Sound Source Holder");
	  soundSourceHolder.transform.position = Vector3.zero;
	  _pool = new AudioSource[_initialSize];
	  for (int i = 0; i < _initialSize; i++)
	  {

		 GameObject audioGo = new GameObject("AudioSourceOb " + i);
		 _pool[i] = audioGo.AddComponent(typeof(AudioSource)) as AudioSource;

		 audioGo.transform.parent = soundSourceHolder.transform;
		 availableSources.Enqueue(_pool[i]);
	  }
   }


   public void PlaySFXSound(AudioDataSO audioData, Vector3 position, Transform parentTransform = null)
   {
	  if (audioData != null)
	  {
		 AudioClip[] clipsToPlay = audioData.clipData.GetClips();
		 //SoundEmitter[] soundEmitterArray = new SoundEmitter[clipsToPlay.Length];

		 int nOfClips = clipsToPlay.Length;
		 for (int i = 0; i < nOfClips; i++)
		 {

			var curSource = GetAvailableSource();
			if (curSource != null)
			{

			   curSource.clip = clipsToPlay[i];

			   curSource.volume = audioData.volume;
			   curSource.pitch = audioData.pitch;


			   if (audioData.is3D)
			   {
				  curSource.spatialBlend = 1;
				  curSource.minDistance = audioData.minDist;
				  curSource.maxDistance = audioData.maxDist;
				  curSource.rolloffMode = AudioRolloffMode.Linear;
			   }
			   else
			   {
				  curSource.spatialBlend = 0;
			   }

			   curSource.loop = audioData.isLooping;




			   curSource.transform.position = position;

			   if (parentTransform != null)
			   {
				  curSource.transform.position = Vector3.zero;
				  curSource.transform.parent = parentTransform;
				  curSource.transform.localPosition = Vector3.zero;
			   }

			   curSource.Play();

			   currentlyPlaying.Add(curSource);
			}
		 }
	  }
   }



   private AudioSource GetAvailableSource()
   {
	  if (availableSources.Count == 0) { return null; }

	  AudioSource returnSource = availableSources.Dequeue();

	  if (returnSource == null) { return null; }

	  return returnSource;
   }

   private void ResetAudioSource(AudioSource reset)
   {
	  reset.clip = null;
	  reset.pitch = 1;
	  reset.volume = 1;
	  reset.loop = false;
	  reset.minDistance = 1.0f;
	  reset.maxDistance = 500.0f;
	  reset.spatialBlend = 1.0f;


	  reset.transform.parent = soundSourceHolder.transform;
	  reset.transform.position = Vector3.zero;
	  availableSources.Enqueue(reset);
   }



   /*// Play a single clip through the sound effects source.
   public void Play2D(AudioDataSO data)
   {

	   AudioSource effectSource = EmptyEffectsSource();

	   effectSource.pitch = data.pitch;

	   effectSource.clip = data.clipData.GetClips()[0];

	   effectSource.Play();
   }

   // Play a single clip through the music source.
   public void PlayMusic2D(AudioClip clip)
   {
	   MusicSource.clip = clip;
	   MusicSource.Play();
   }


   public void RandomSoundEffect2D(params AudioClip[] clips)
   {
	   RandomSoundFromList(clips, 1, 1);
   }

   // Play a random clip from an array, and randomize the pitch slightly.
   public void RandomSoundFromList(AudioClip[] clips, float lowPitch = 1, float highPitch = 1)
   {
	   int randomIndex = Random.Range(0, clips.Length);
	   float randomPitch = Random.Range(lowPitch, highPitch);

	   AudioSource effectSource = EmptyEffectsSource();

	   effectSource.pitch = randomPitch;
	   effectSource.clip = clips[randomIndex];
	   effectSource.Play();
   }
   */




}
