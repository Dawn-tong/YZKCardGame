using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType {
	None,
	Normal,
	Bomb,
}

[System.Serializable]
public class Card {
	//这些字段需要是public的，用于序列化
	public bool exists;

	[HideInInspector] public Chess chessComponent;	//对应的棋子组件
	public bool isSelfCard;		//是自己的才可以移动
	public bool isVisible;		//是可被看到属性的
	public bool isFought;		//是战斗过的
	
	public int index = -1;
	public CardType cardType;
	public int level;	// 星级
	public int hp;		// 生命
	public int atk;		// 攻击
	public int positionX = -1;
	public int positionY = -1;

	public Card(){
		exists = false;
		index = -1;
		cardType = CardType.None;
		level = 0;
		hp = 0;
		atk = 0;
		positionX = -1;
		positionY = -1;
		isFought = false;
	}

	public Card(int index, CardType cardType, int level, int hp, int atk, int positionX, int positionY){
		if(index < 0 || index >= CardsListManager.MAX_TOTAL_CARDS) {
			this.exists = false;
			return;
		}
		this.exists = true;
		this.index = index;
		this.cardType = cardType;
		this.level = level;
		this.hp = hp;
		this.atk = atk;
		this.positionX = positionX;
		this.positionY = positionY;
		this.isFought = false;
	}

	public Card(Card card){
		this.exists = card.exists;
		this.isSelfCard = card.isSelfCard;
		this.isVisible = card.isVisible;
		this.index = card.index;
		this.cardType = card.cardType;
		this.level = card.level;
		this.hp = card.hp;
		this.atk = card.atk;
		this.positionX = card.positionX;
		this.positionY = card.positionY;
		this.isFought = card.isFought;
	}

	public override string ToString() {
		return $"卡牌[{index}]:类型={cardType}; 星级={level}; HP={hp}; ATK={atk}; 坐标=({positionX},{positionY});";
	}

	//显示卡牌信息
	public string CardInfoToString() {
		if(cardType == CardType.Normal) {
			return $"星级 = {level}; 生命 = {hp}; 攻击 = {atk}";
		}
		else if(cardType == CardType.Bomb) {
			return $"炸弹卡";
		}
		return "空卡";
	}
}
