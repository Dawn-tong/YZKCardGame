using UnityEngine;

public class CardSettingUI_OneCardPanel : MonoBehaviour {
    public GameObject createCardPanel;
    public GameObject propretiesPanel;
    public void SetOneCardPanel(int cardIndex) {
		//设置要显示的内容
		cardInfo.CardIndex = cardIndex;
        if(cardInfo.Card == null) {
            createCardPanel.SetActive(true);
            propretiesPanel.SetActive(false);
        } else {
            createCardPanel.SetActive(false);
            propretiesPanel.SetActive(true);
        }
    }






    CardSettingCardInfo cardInfo;
    CardManager cardManager;
    void Awake(){
        cardInfo = GetComponent<CardSettingCardInfo>();
        cardManager = PlayerManager.Instance.currentPlayer.cardManager;
    }
    public void ClickButtonToCreateCard() {
        Debug.Log("按钮 - 创建卡片");
        if(cardManager.AddNormalCard(cardInfo.CardIndex)) {
            Debug.Log("卡片创建成功");
            cardInfo.UpdateCardInfo();
            createCardPanel.SetActive(false);
            propretiesPanel.SetActive(true);
        }
        else {
            Debug.Log("卡片创建失败");
        }
    }
    public void ClickButtonToDeleteCard() {
        Debug.Log("按钮 - 删除卡片");
        if(cardManager.DeleteCard(cardInfo.CardIndex)) {
            Debug.Log("卡片删除成功");
            cardInfo.UpdateCardInfo();
            createCardPanel.SetActive(true);
            propretiesPanel.SetActive(false);
        }
        else {
            Debug.Log("卡片删除失败");
        }
    }
    public void ClickButtonToLevelUp() {
        Debug.Log("按钮 - 升级卡片");
        cardManager.CardLevelUp(cardInfo.CardIndex);
		cardInfo.UpdateCardInfo();
    }
    public void ClickButtonToLevelDown() {
        Debug.Log("按钮 - 降级卡片");
        cardManager.CardLevelDown(cardInfo.CardIndex);
        cardInfo.UpdateCardInfo();
    }
    public void ClickButtonToHpUp() {
        Debug.Log("按钮 - 增加血量");
        cardManager.CardHpUp(cardInfo.CardIndex);
        cardInfo.UpdateCardInfo();
    }
    public void ClickButtonToHpDown() {
        Debug.Log("按钮 - 减少血量");
        cardManager.CardAtkUp(cardInfo.CardIndex);
        cardInfo.UpdateCardInfo();
    }
    public void ClickButtonToAtkUp() {
        Debug.Log("按钮 - 增加攻击力");
        cardManager.CardAtkUp(cardInfo.CardIndex);
        cardInfo.UpdateCardInfo();
    }
    public void ClickButtonToAtkDown() {
        Debug.Log("按钮 - 减少攻击力");
        cardManager.CardHpUp(cardInfo.CardIndex);
        cardInfo.UpdateCardInfo();
    }






	public CardSettingUI_AllCardsPanel allCardsPanel;
	public void ShowAllCardsPanel() {
		gameObject.SetActive(true);
	}
	public void HideAllCardsPanel() {
		gameObject.SetActive(false);
		allCardsPanel.ShowAllCardsPanel();
	}
}
