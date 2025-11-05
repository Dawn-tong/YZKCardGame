using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICardSetting : MonoBehaviour
{
	public GameObject scenePanel;
	public GameObject allCardsPanel;
	public GameObject oneCardPanel;

	public void OnSceneEnter() {

		
	}
	//scene Panel
	public void ClickButtonToGoBackTitle() {
		Debug.Log("Button - Go Back To Title Scene");
		SceneLoaderManager.Instance.LoadScene(Scene.TitleScene);
	}
	public void ClickButtonToShowAllCardsPanel() {
		Debug.Log("Button - Show All Cards Panel");
		allCardsPanel.SetActive(true);
	}

	//all Cards Panel
	public void ClickButtonToHideAllCardsPanel() {
		Debug.Log("Button - Hide All Cards Panel");
		allCardsPanel.SetActive(false);
	}

	//one Card Panel
	public void ClickButtonToHideOneCardPanel() {
		Debug.Log("Button - Hide One Card Panel");
	}
}
