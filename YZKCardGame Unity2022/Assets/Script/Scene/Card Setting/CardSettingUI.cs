using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSettingUI : MonoBehaviour
{
	public GameObject scenePanel;
	public GameObject allCardsPanel;
	public Transform cardContent;
	public GameObject cardPrefab;
	public GameObject oneCardPanel;
	void Start() {
		cardPrefab.SetActive(false);
	}





	public void ClickButtonToGoBackTitle() {
		//保存卡牌到本地
		PlayerManager.Instance.currentPlayer.cardManager.SaveCardsListToLocal();
		Debug.Log("按钮 - 保存卡组并返回主界面");
		SceneLoaderManager.Instance.LoadScene(Scene.TitleScene);
	}
	public void ClickButtonToShowAllCardsPanel() {
		Debug.Log("按钮 - 显示所有卡片");
		allCardsPanel.SetActive(true);
		UpdateAllCardsPanel();
	}
	public void ClickButtonToHideAllCardsPanel() {
		Debug.Log("按钮 - 隐藏所有卡片");
		allCardsPanel.SetActive(false);
	}
	public void ClickButtonToHideOneCardPanel() {
		Debug.Log("按钮 - 隐藏卡片详情");
	}






	List<GameObject> cardObjects = new List<GameObject>();
	void UpdateAllCardsPanel() {
		foreach (GameObject cardObj in cardObjects) {
			Destroy(cardObj);
		}
		cardObjects.Clear();

		Card[] cardsList = PlayerManager.Instance.currentPlayer.cardManager.cardsList;
		for (int i = 0; i <= PlayerManager.Instance.currentPlayer.cardManager.lastNormalCardIndex; i++) {
			GameObject cardObj = Instantiate(cardPrefab, cardContent);
			cardObj.GetComponent<CardSettingCardInfo>().card = cardsList[i];
			Button button = cardObj.GetComponent<Button>();
            button.onClick.AddListener(() => {
                ShowOneCardPanel(i);
            });
			cardObj.SetActive(true);
			cardObjects.Add(cardObj);
		}
	}
	//查看卡片详情
	public void ShowOneCardPanel(int index) {
		Debug.Log($"按钮 - 显示卡片详情,列表ID={index}");
		oneCardPanel.SetActive(true);
	}

}
