using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITitle : MonoBehaviour
{
	public Text titleText;
	public Button gameStartButton;
	public Button CardsSetButton;
	public Button gameExitButton;

	public void ClickButtonToGameStart() {
		Debug.Log("Button - Game Start");
		SceneLoaderManager.Instance.LoadScene(Scene.HallScene);
	}
	public void ClickButtonToCardsSetScene() {
		Debug.Log("Button - Load Cards Set Scene");
		SceneLoaderManager.Instance.LoadScene(Scene.CardSetting);
	}
	public void ClickButtonToExitGame() {
		Debug.Log("Button - Game Exit");
		Application.Quit();
	}
}
