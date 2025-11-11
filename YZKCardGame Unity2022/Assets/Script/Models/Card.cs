using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType {
	Normal,
	Bomb,
}

[System.Serializable]
public class Card {
	public bool exists;
	//这些字段需要是public的，用于序列化
	public int index = -1;
	public CardType cardType;
	public int level;   // 星级
	public int hp;		// 生命
	public int atk;		// 攻击
	public int positionX = -1;
	public int positionY = -1;

	public override string ToString() {
		return $"卡牌 [{cardType}] 星级:{level} HP:{hp} ATK:{atk}";
	}
}
