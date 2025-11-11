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

	public void ClickButtonToRuleScene() {
		Debug.Log("按钮 - 加载规则场景");
		SceneLoaderManager.Instance.LoadScene(Scene.RuleScene);
	}
	public void ClickButtonToCardsSetScene() {
		Debug.Log("按钮 - 加载卡牌设置场景");
		SceneLoaderManager.Instance.LoadScene(Scene.CardSetting);
	}
	public void ClickButtonToGameStart() {
		Debug.Log("按钮 - 游戏开始");
		SceneLoaderManager.Instance.LoadScene(Scene.HallScene);
	}
	public void ClickButtonToExitGame() {
		Debug.Log("按钮 - 退出游戏");
		Application.Quit();
	}
}
