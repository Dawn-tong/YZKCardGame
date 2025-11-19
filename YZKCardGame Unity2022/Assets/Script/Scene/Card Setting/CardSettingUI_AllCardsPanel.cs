using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CardSettingUI_AllCardsPanel : MonoBehaviour {
	void Start() {
		cardPrefab.SetActive(false);
	}






	public GameObject cardPrefab;
	public Transform cardContent;
	List<GameObject> allContents = new List<GameObject>();
	void UpdateAllCardsPanel() {
		foreach (GameObject cardObj in allContents) {
			Destroy(cardObj);
		}
		allContents.Clear();

		Card[] cardsList = PlayerManager.Instance.currentPlayer.cardManager.cardsList;
		for (int i = 0; i < cardsList.Length; i++) {
			//创建内容
			GameObject cardObj = Instantiate(cardPrefab, cardContent);
			allContents.Add(cardObj);
			cardObj.SetActive(true);
			//设置要显示的内容
			CardSettingCardInfo cardInfo = cardObj.GetComponent<CardSettingCardInfo>();
			cardInfo.CardIndex = i;
			//设置回调
			Button button = cardObj.GetComponent<Button>();
			int idx = i;//这句不写有错误
            button.onClick.AddListener(
				() => { 
					ShowOneCardPanel(cardsList[idx], idx);
				}
			);
		}
	}
	//查看卡片详情
	public CardSettingUI_OneCardPanel oneCardPanel;
	public void ShowOneCardPanel(Card card, int cardIndex) {
		Debug.Log($"按钮 - 显示卡片详情");
		oneCardPanel.ShowOneCardsPanel();
		oneCardPanel.UpdateOneCardPanel(cardIndex);
		oneCardPanel.CloseCallback += ShowAllCardsPanel;
		HideAllCardsPanel();
	}






	public delegate void ShowDelegate();
	public event ShowDelegate ShowCallback;
	public void ShowAllCardsPanel() {
		oneCardPanel.CloseCallback -= ShowAllCardsPanel;
		gameObject.SetActive(true);
		UpdateAllCardsPanel();
		ShowCallback?.Invoke();
	}
	public void HideAllCardsPanel() {
		gameObject.SetActive(false);
	}
}
