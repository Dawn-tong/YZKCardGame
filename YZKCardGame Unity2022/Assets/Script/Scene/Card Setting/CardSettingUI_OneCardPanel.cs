using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSettingUI_OneCardPanel : MonoBehaviour {
    CardManager cardManager;
    void Start() {
        cardManager = PlayerManager.Instance.currentPlayer.cardManager;
    }





    
    public void ClickButtonToLevelUp() {
        Debug.Log("按钮 - 升级卡片");
        CardSettingCardInfo cardInfo = GetComponent<CardSettingCardInfo>();
        cardManager.CardLevelUp(cardInfo.card.cardIndex);
		cardInfo.UpdateCardInfo();
    }
    public void ClickButtonToLevelDown() {
        Debug.Log("按钮 - 降级卡片");
        CardSettingCardInfo cardInfo = GetComponent<CardSettingCardInfo>();
        cardManager.CardLevelDown(cardInfo.card.cardIndex);
        cardInfo.UpdateCardInfo();
    }
    public void ClickButtonToHpUp() {
        Debug.Log("按钮 - 增加血量");
        CardSettingCardInfo cardInfo = GetComponent<CardSettingCardInfo>();
        cardManager.CardHpUp(cardInfo.card.cardIndex);
        cardInfo.UpdateCardInfo();
    }
    public void ClickButtonToHpDown() {
        Debug.Log("按钮 - 减少血量");
        CardSettingCardInfo cardInfo = GetComponent<CardSettingCardInfo>();
        cardManager.CardAtkUp(cardInfo.card.cardIndex);
        cardInfo.UpdateCardInfo();
    }
    public void ClickButtonToAtkUp() {
        Debug.Log("按钮 - 增加攻击力");
        CardSettingCardInfo cardInfo = GetComponent<CardSettingCardInfo>();
        cardManager.CardAtkUp(cardInfo.card.cardIndex);
        cardInfo.UpdateCardInfo();
    }
    public void ClickButtonToAtkDown() {
        Debug.Log("按钮 - 减少攻击力");
        CardSettingCardInfo cardInfo = GetComponent<CardSettingCardInfo>();
        cardManager.CardHpUp(cardInfo.card.cardIndex);
        cardInfo.UpdateCardInfo();
    }
}
