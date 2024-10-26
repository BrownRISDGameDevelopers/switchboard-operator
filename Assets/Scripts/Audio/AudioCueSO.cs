using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Audio/Audio Data")]
public class AudioCueSO : ScriptableObject
{
	public AudioDataSO clipData;
	public bool is3D;
	public bool isLooping;

	[Range(0, 1)]
	public float volume = 1.0f;
	[Range(0, 2)]
	public float pitch = 1.0f;

	public float minDist = 1.0f;
	public float maxDist = 500.0f;
}
