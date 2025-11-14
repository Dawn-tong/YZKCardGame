using UnityEngine;
using UnityEngine.UI;

public class CardSettingUI : MonoBehaviour {
	public Text LevelSumText;
	CardManager cardManager;

	void Awake() {
		SceneLoaderManager.Instance.RegisterSceneEnterCallback(Scene.CardSetting, OnSceneEnter);
		SceneLoaderManager.Instance.RegisterSceneLeaveCallback(Scene.CardSetting, OnSceneLeave);
	}
	public void OnSceneEnter() {
		cardManager = PlayerManager.Instance.currentPlayer.cardManager;
		//订阅星级修改
		cardManager.CardLevelChanged += UpdateLevelSumText;
		UpdateLevelSumText();
		//设置需要用到的卡组
		CardSettingCardInfo.cardsList = cardManager.cardsList;
	}
	public void OnSceneLeave() {
		SceneLoaderManager.Instance.UnregisterSceneEnterCallback(Scene.CardSetting, OnSceneEnter);
		SceneLoaderManager.Instance.UnregisterSceneLeaveCallback(Scene.CardSetting, OnSceneLeave);
		cardManager.CardLevelChanged -= UpdateLevelSumText;
	}
	void UpdateLevelSumText() {
		LevelSumText.text = $"卡牌总星级：{cardManager.CalSumOfCardsLevel()} / 55";
	}






	public void ClickButtonToGoBackTitle() {
		//保存卡牌到本地
		PlayerManager.Instance.currentPlayer.cardManager.SaveCardsListToLocal();
		Debug.Log("按钮 - 保存卡组并返回主界面");
		SceneLoaderManager.Instance.LoadScene(Scene.TitleScene);
	}
	
	public CardSettingUI_AllCardsPanel allCardsPanel;
	public void ClickButtonToShowAllCardsPanel() {
		Debug.Log("按钮 - 显示所有卡片");
		allCardsPanel.ShowAllCardsPanel();
	}

	public CardSettingBoard cardSettingBoard;
	public void ClickButtonToResetAllCards() {
		UIManager.Instance.CreateUI<UIPopup>().InitUIPopup("重置卡牌", "确认重置所有卡牌" 
			,(result) => {
				if (result) {
					cardManager.InitCardsList();
					UpdateLevelSumText();
					cardSettingBoard.UpdateBoard();
				}
			}
		);
	}
}
