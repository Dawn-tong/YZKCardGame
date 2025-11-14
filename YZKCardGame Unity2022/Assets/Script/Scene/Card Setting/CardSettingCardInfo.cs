using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSettingCardInfo : MonoBehaviour {
	[Header("编号")]
	[SerializeField] Text indexText;
	
	[Header("普通卡牌")]
	[SerializeField] GameObject normalPanel;
	[SerializeField] Text LevelText;
	[SerializeField] Text HpText;
	[SerializeField] Text AtkText;
	
	[Header("特殊卡牌")]
	[SerializeField] GameObject specialPanel;
	[SerializeField] Text SpecialText;
	
	
	
	
	
	
	//当前显示的卡片
	public Card Card { private set; get; }
	//当前玩家的卡牌列表，用于刷新当前卡片
	public static Card[] cardsList;
	int cardIndex;
	/// <summary>
	/// 刷新显示内容
	/// </summary>
	public int CardIndex {
		set { 
			cardIndex = value;
			UpdateCardInfo();
		}
		get {
			return cardIndex;
		}
	}
	public void UpdateCardInfo() {
		indexText.text = (CardIndex + 1).ToString();
		Card = cardsList[CardIndex];
		if(Card == null) {
			normalPanel.SetActive(true);
			specialPanel.SetActive(false);
			LevelText.text = "--";
			HpText.text = "--";
			AtkText.text = "--";
			return;
		}
		if (Card.cardType == CardType.Normal) {
			normalPanel.SetActive(true);
			specialPanel.SetActive(false);
			LevelText.text = Card.level.ToString();
			HpText.text = Card.hp.ToString();
			AtkText.text = Card.atk.ToString();
		}
		else if (Card.cardType == CardType.Bomb) {
			normalPanel.SetActive(false);
			specialPanel.SetActive(true);
			SpecialText.text = "炸弹";
		}
	}
}
