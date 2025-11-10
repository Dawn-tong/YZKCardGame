using UnityEngine;
using UnityEngine.UI;

public class CardSettingUI_OneCardPanel : MonoBehaviour {
    //初始化
    CardSettingCardInfo cardInfo;
    CardManager cardManager;
    void Awake(){
        cardInfo = GetComponent<CardSettingCardInfo>();
        cardManager = PlayerManager.Instance.currentPlayer.cardManager;
    }
    //更新单张卡片面板
    public GameObject createCardPanel;
    public GameObject propretiesPanel;
    public void UpdateOneCardPanel(int cardIndex) {
    	//设置要显示的内容
    	cardInfo.CardIndex = cardIndex;
        Card card = cardInfo.Card;
        if(card == null) {
            createCardPanel.SetActive(true);
            propretiesPanel.SetActive(false);
        }
        else {
            createCardPanel.SetActive(false);
            propretiesPanel.SetActive(true);
        }
        UpdateButtonState();
    }
    //显示隐藏面板
    public CardSettingUI_AllCardsPanel allCardsPanel;
    public void ShowOneCardsPanel() {
    	gameObject.SetActive(true);
    }
    public void HideOneCardsPanel() {
    	gameObject.SetActive(false);
    }






	//将卡片放置在棋盘上
	[SerializeField] CardSettingBoard cardSettingBoard;
	public void ClickButtonToCreateCard() {
        //Debug.Log("按钮 - 创建卡片");
        if(cardManager.AddNormalCard(cardInfo.CardIndex)) {
            //Debug.Log("卡片创建成功");
            cardInfo.UpdateCardInfo();
            createCardPanel.SetActive(false);
            propretiesPanel.SetActive(true);
            UpdateButtonState();
        }
    }
    public void ClickButtonToDeleteCard() {
        //Debug.Log("按钮 - 删除卡片");
        int positionX = cardInfo.Card.positionX;
        int positionY = cardInfo.Card.positionY;
        if(cardManager.DeleteCard(cardInfo.CardIndex)) {
            //Debug.Log("卡片删除成功");
            cardInfo.UpdateCardInfo();
            createCardPanel.SetActive(true);
            propretiesPanel.SetActive(false);
            UpdateButtonState();
            cardSettingBoard.UpdateBoardChess(positionX, positionY);
        }
    }
    public void ClickButtonToLevelUp() {
        //Debug.Log("按钮 - 升级卡片");
        cardManager.CardLevelUp(cardInfo.CardIndex);
    	cardInfo.UpdateCardInfo();
        UpdateButtonState();
        cardSettingBoard.UpdateBoardChess(cardInfo.Card.positionX, cardInfo.Card.positionY);
    }
    public void ClickButtonToLevelDown() {
        //Debug.Log("按钮 - 降级卡片");
        cardManager.CardLevelDown(cardInfo.CardIndex);
        cardInfo.UpdateCardInfo();
        UpdateButtonState();
        cardSettingBoard.UpdateBoardChess(cardInfo.Card.positionX, cardInfo.Card.positionY);
    }
    public void ClickButtonToHpUp() {
        //Debug.Log("按钮 - 增加血量");
        cardManager.CardHpUp(cardInfo.CardIndex);
        cardInfo.UpdateCardInfo();
        UpdateButtonState();
        cardSettingBoard.UpdateBoardChess(cardInfo.Card.positionX, cardInfo.Card.positionY);
    }
    public void ClickButtonToHpDown() {
        //Debug.Log("按钮 - 减少血量");
        cardManager.CardAtkUp(cardInfo.CardIndex);
        cardInfo.UpdateCardInfo();
        UpdateButtonState();
        cardSettingBoard.UpdateBoardChess(cardInfo.Card.positionX, cardInfo.Card.positionY);
    }
    public void ClickButtonToAtkUp() {
        //Debug.Log("按钮 - 增加攻击力");
        cardManager.CardAtkUp(cardInfo.CardIndex);
        cardInfo.UpdateCardInfo();
        UpdateButtonState();
        cardSettingBoard.UpdateBoardChess(cardInfo.Card.positionX, cardInfo.Card.positionY);
    }
    public void ClickButtonToAtkDown() {
        //Debug.Log("按钮 - 减少攻击力");
        cardManager.CardHpUp(cardInfo.CardIndex);
        cardInfo.UpdateCardInfo();
        UpdateButtonState();
        cardSettingBoard.UpdateBoardChess(cardInfo.Card.positionX, cardInfo.Card.positionY);
    }
    public void ClickButtonToReturnToAllCardsPanel() {
        HideOneCardsPanel();
    	allCardsPanel.ShowAllCardsPanel();
    }






    //更新按钮状态
    [SerializeField] Button CreateCardButton;
    [SerializeField] Button DeleteCardButton;
    [SerializeField] Button LevelUpButton;
    [SerializeField] Button LevelDownButton;
    [SerializeField] Button HpUpButton;
    [SerializeField] Button HpDownButton;
    [SerializeField] Button AtkUpButton;
    [SerializeField] Button AtkDownButton;
    public void UpdateButtonState() {
        Card card = cardInfo.Card;
        if(card == null) {
            if(cardManager.CalSumOfCardsLevel()< cardManager.maxTotalLevel) {
                ShowButton(CreateCardButton);
            } 
            else {
                HideButton(CreateCardButton);
            }
            return;
        }
        if(card.cardType != CardType.Normal) {
            HideButton(DeleteCardButton);
            HideButton(LevelUpButton);
            HideButton(LevelDownButton);
            HideButton(HpUpButton);
            HideButton(HpDownButton);
            HideButton(AtkUpButton);
            HideButton(AtkDownButton);
            return;
        }
        //如果非最大等级且等级总和未超过上限
        if(card.level < cardManager.cardMaxLevel && cardManager.CalSumOfCardsLevel()< cardManager.maxTotalLevel) {
            ShowButton(LevelUpButton);
        } 
        else {
            HideButton(LevelUpButton);
        }
        //如果等级大于1
        if(card.level > 1) {
            ShowButton(LevelDownButton);
        } 
        else {
            HideButton(LevelDownButton);
        }
        //如果血量>1
        if(card.hp > 1) {
            ShowButton(HpDownButton);
            ShowButton(AtkUpButton);
        } 
        else {
            HideButton(HpDownButton);
            HideButton(AtkUpButton);
        }
        //如果攻击力>1
        if(card.atk > 1) {
            ShowButton(AtkDownButton);
            ShowButton(HpUpButton);
        } 
        else {
            HideButton(AtkDownButton);
            HideButton(HpUpButton);
        }
    }
    //显示隐藏按钮
    public void ShowButton(Button button) {
        Text text = button.GetComponentInChildren<Text>();
        text.color = Color.white;
    }
    public void HideButton(Button button) {
        Text text = button.GetComponentInChildren<Text>();
        text.color = new Color(0.25f, 0.25f, 0.25f);
    }






    public void ClickButtonToPutCardOnBoard() {
        cardSettingBoard.OnTileClicked += PutCardOnBoard;
        SceneLoaderManager.Instance.RegisterSceneLeaveCallback(Scene.CardSetting, CancelListenerPutEvent);
        //隐藏单张卡片面板
        HideOneCardsPanel();
    }
    //取消监听放置事件
    public void CancelListenerPutEvent() {
        cardSettingBoard.OnTileClicked -= PutCardOnBoard;
        SceneLoaderManager.Instance.UnregisterSceneLeaveCallback(Scene.CardSetting, CancelListenerPutEvent);
    }
    //放置卡片
    void PutCardOnBoard(int positionX, int positionY) {
        Debug.Log($"放置卡片在{positionX},{positionY}");
        cardManager.PutCardToPosition(cardInfo.Card, positionX, positionY);
        //取消监听放置事件
        CancelListenerPutEvent();
        //显示单张卡片面板
        ShowOneCardsPanel();
    }
}
