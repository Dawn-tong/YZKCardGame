using UnityEngine;
using UnityEngine.UI;

public class CardSettingUI : MonoBehaviour {
	public Text LevelSumText;
	CardManager cardManager;
	public void OnSceneEnter() {
		cardManager = PlayerManager.Instance.currentPlayer.cardManager;
		//订阅星级修改
		cardManager.CardLevelChanged += UpdateLevelSumText;
		UpdateLevelSumText();
		//设置需要用到的卡组
		CardSettingCardInfo.cardsList = cardManager.cardsList;
		//刷新按钮状态
		UpdatePresetCardsButtonState();
	}
	public void OnSceneLeave() {
		cardManager.CardLevelChanged -= UpdateLevelSumText;
	}
	void UpdateLevelSumText() {
		LevelSumText.text = $"卡牌总星级：{cardManager.CalSumOfCardsLevel()} / 55";
	}






	public Button presetCardsButton1;
	public Button presetCardsButton2;
	public Button presetCardsButton3;
	public Button presetCardsButton4;
	public Button presetCardsButton5;
	public void ClickButtonToUsePresetCards1() {
		CardsStorageManager.SwitchCardSet(0);
		UpdatePresetCardsButtonState();
		cardSettingBoard.UpdateBoard();
	}
	public void ClickButtonToUsePresetCards2() {
		CardsStorageManager.SwitchCardSet(1);
		UpdatePresetCardsButtonState();
		cardSettingBoard.UpdateBoard();
	}
	public void ClickButtonToUsePresetCards3() {
		CardsStorageManager.SwitchCardSet(2);
		UpdatePresetCardsButtonState();
		cardSettingBoard.UpdateBoard();
	}
	public void ClickButtonToUsePresetCards4() {
		CardsStorageManager.SwitchCardSet(3);
		UpdatePresetCardsButtonState();
		cardSettingBoard.UpdateBoard();
	}
	public void ClickButtonToUsePresetCards5() {
		CardsStorageManager.SwitchCardSet(4);
		UpdatePresetCardsButtonState();
		cardSettingBoard.UpdateBoard();
	}
	void UpdatePresetCardsButtonState() {
		presetCardsButton1.interactable = CardsStorageManager.CurrentCardSetIndex != 0;
		presetCardsButton2.interactable = CardsStorageManager.CurrentCardSetIndex != 1;
		presetCardsButton3.interactable = CardsStorageManager.CurrentCardSetIndex != 2;
		presetCardsButton4.interactable = CardsStorageManager.CurrentCardSetIndex != 3;
		presetCardsButton5.interactable = CardsStorageManager.CurrentCardSetIndex != 4;
		presetCardsButton1.GetComponent<Image>().color = CardsStorageManager.CurrentCardSetIndex == 0 ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
		presetCardsButton2.GetComponent<Image>().color = CardsStorageManager.CurrentCardSetIndex == 1 ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
		presetCardsButton3.GetComponent<Image>().color = CardsStorageManager.CurrentCardSetIndex == 2 ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
		presetCardsButton4.GetComponent<Image>().color = CardsStorageManager.CurrentCardSetIndex == 3 ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
		presetCardsButton5.GetComponent<Image>().color = CardsStorageManager.CurrentCardSetIndex == 4 ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
	}







	public void ClickButtonToGoBackTitle() {
		//保存卡牌到本地
		CardsStorageManager.SaveCardsListToLocal();
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
		UIManager.Instance.CreateUI<UIPopup>().InitUIPopup("重置卡牌", "重置所有卡牌并使用预设卡组" 
			,(result) => {
				if (result) {
					CardsStorageManager.ResetToPresetCards();
					cardSettingBoard.UpdateBoard();
				}
			}
		);
	}
}
