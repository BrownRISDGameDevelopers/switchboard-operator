using UnityEngine;
using UnityEngine.UI;

public class TutorialMenu : MonoBehaviour
{
	public GameObject tutorialScreens;
	public Button prevButton;
	public Button nextButton;
	public Button exitButton;

	private int currentScreen = 0;

	void Start()
	{
		// Ensure all screens except the first are deactivated
		for (int i = 0; i < tutorialScreens.transform.childCount; i++)
		{
			tutorialScreens.transform.GetChild(i).gameObject.SetActive(i == 0);
		}

		prevButton.onClick.AddListener(ShowPreviousScreen);
		nextButton.onClick.AddListener(ShowNextScreen);
		exitButton.onClick.AddListener(OnExit);

		UpdateButtonStates();
	}

	void OnExit()
	{
		Destroy(this.gameObject);
	}

	void ShowNextScreen()
	{
		if (currentScreen < tutorialScreens.transform.childCount - 1)
		{
			tutorialScreens.transform.GetChild(currentScreen).gameObject.SetActive(false);
			currentScreen++;
			tutorialScreens.transform.GetChild(currentScreen).gameObject.SetActive(true);

			UpdateButtonStates();
		}
	}

	void ShowPreviousScreen()
	{
		if (currentScreen > 0)
		{
			tutorialScreens.transform.GetChild(currentScreen).gameObject.SetActive(false);
			currentScreen--;
			tutorialScreens.transform.GetChild(currentScreen).gameObject.SetActive(true);

			UpdateButtonStates();
		}
	}

	void UpdateButtonStates()
	{
		prevButton.interactable = currentScreen > 0;
		nextButton.interactable = currentScreen < tutorialScreens.transform.childCount - 1;
	}
}