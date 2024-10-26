using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicators : MonoBehaviour
{
	public GameObject strikes;
	public GameObject activeCalls;

	void Start()
	{
		Transform strike = strikes.transform.Find("1");
		Image strikeImage = strike.GetComponent<Image>();
		strikeImage.color = Color.red;
	}
}