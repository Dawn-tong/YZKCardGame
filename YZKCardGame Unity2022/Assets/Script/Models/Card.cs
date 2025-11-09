using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType {
	Normal,
	Bomb,
}

[System.Serializable]
public class Card {
	//这些字段需要是public的，用于序列化
	public CardType cardType;
	public int level;   // 星级
	public int hp;		// 生命
	public int atk;		// 攻击
	public int positionX;
	public int positionY;

	public override string ToString() {
		return $"卡牌 [{cardType}] 星级:{level} HP:{hp} ATK:{atk}";
	}
}
