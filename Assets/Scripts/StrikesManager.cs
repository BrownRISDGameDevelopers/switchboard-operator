using UnityEngine;
using System.Collections;

public class StrikeManager : MonoBehaviour
{
   public GameObject[] strikeLights;
   private int strikeCount = 0;
   private const int maxStrikes = 3;

   void Start()
   {
	  ResetStrikes();
	  // print("Starting");
	  // StartCoroutine(ResetStrikesAfterDelay());
	  // print("YAY");
   }

   private void OnStrike(int strikesLeft, bool recharge)
   {
	  for (int i = 0; i < Mathf.Max(0, maxStrikes - strikesLeft); i++)
	  {
		 strikeLights[i].SetActive(true);
	  }
   }

   public void RegisterStrike()
   {
	  strikeCount++;
	  strikeLights[strikeCount - 1].SetActive(true);

	  if (strikeCount >= maxStrikes)
	  {
		 // do something to end the game
	  }
   }

   public void ResetStrikes()
   {
	  strikeCount = 0;
	  foreach (var light in strikeLights)
	  {
		 light.SetActive(false);
	  }
   }

   private IEnumerator ResetStrikesAfterDelay()
   {
	  yield return new WaitForSeconds(3);
	  print("Striking");
	  RegisterStrike();
   }
}
