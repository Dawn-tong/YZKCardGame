using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneUI : MonoBehaviour {
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
	bool canEnterGame = false;
	public void ClickButtonToGameStart() {
		Debug.Log("按钮 - 游戏开始");
		if (canEnterGame) {
			SceneLoaderManager.Instance.LoadScene(Scene.HallScene);
		} else {
			Debug.Log("创建提示窗口");
			UIManager.Instance.CreateUI<UIMessage>().InitUIMessage("提示", "请先设置卡组");
		}
	}
	public void ClickButtonToExitGame() {
		Debug.Log("按钮 - 退出游戏");
		Application.Quit();
	}






	public void OnSceneEnter() {
		//判断是否有卡牌放置在棋盘上
		canEnterGame = false;
		Card[] cards = PlayerManager.Instance.currentPlayer.currentCardManager.cardsList;
		foreach (Card card in cards) {
			if (card != null && card.positionX != -1) {
				canEnterGame = true;
				break;
			}
		}
	}
}
