using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetCardsList {
	public static Card[][] presetCardsList = new Card[5][];
	public static Card[] GetPresetCardsList(int index) {
		switch (index) {
			case 0:
				return PresetCardsList0();
			case 1:
				return PresetCardsList1();
			case 2:
				return PresetCardsList2();
			case 3:
				return PresetCardsList3();
			case 4:
				return PresetCardsList4();
			default:
				return null;
		}
	}

	public static Card[] PresetCardsList0() {
		if (presetCardsList[0] == null) {
			Card[] deck = new Card[CardsListManager.MAX_TOTAL_CARDS];
			CreateSpecialCard(deck, 1, CardType.Bomb, 1, 3);
			CreateSpecialCard(deck, 2, CardType.Bomb, 3, 1);
			CreateNormalCard(deck, 3, (1, 1, 1), (2, 3));
			CreateNormalCard(deck, 4, (2, 2, 2), (4, 4));
			CreateNormalCard(deck, 5, (3, 3, 3), (2, 4));
			CreateNormalCard(deck, 6, (4, 4, 4), (3, 2));
			CreateNormalCard(deck, 7, (5, 5, 5), (4, 3));
			CreateNormalCard(deck, 8, (6, 6, 6), (3, 3));
			CreateNormalCard(deck, 9, (7, 7, 7), (4, 1));
			CreateNormalCard(deck, 10, (8, 8, 8), (3, 4));
			CreateNormalCard(deck, 11, (9, 9, 9), (1, 4));
			CreateNormalCard(deck, 12, (10, 10, 10), (4, 2));
			presetCardsList[0] = deck;
		}
		return CloneCardArray(presetCardsList[0]);
	}
	public static Card[] PresetCardsList1() {
		if (presetCardsList[1] == null) {
			Card[] deck = new Card[CardsListManager.MAX_TOTAL_CARDS];
			CreateSpecialCard(deck, 1, CardType.Bomb, 1, 4);
			CreateSpecialCard(deck, 2, CardType.Bomb, 4, 1);
			CreateNormalCard(deck, 3, (1, 1, 1), (4, 2));
			CreateNormalCard(deck, 4, (2, 2, 2), (4, 4));
			CreateNormalCard(deck, 5, (3, 3, 3), (3, 3));
			CreateNormalCard(deck, 6, (4, 4, 4), (4, 3));
			CreateNormalCard(deck, 7, (5, 5, 5), (3, 4));
			CreateNormalCard(deck, 8, (6, 6, 6), (2, 4));
			CreateNormalCard(deck, 9, (7, 7, 7), (3, 1));
			CreateNormalCard(deck, 10, (8, 8, 8), (1, 3));
			CreateNormalCard(deck, 11, (9, 9, 9), (2, 3));
			CreateNormalCard(deck, 12, (10, 10, 10), (3, 2));
			presetCardsList[1] = deck;
		}
		return CloneCardArray(presetCardsList[1]);
	}
	public static Card[] PresetCardsList2() {
		if (presetCardsList[2] == null) {
			Card[] deck = new Card[CardsListManager.MAX_TOTAL_CARDS];
			CreateSpecialCard(deck, 1, CardType.Bomb, 2, 4);
			CreateSpecialCard(deck, 2, CardType.Bomb, 4, 2);
			CreateNormalCard(deck, 3, (1, 1, 1), (3, 1));
			CreateNormalCard(deck, 4, (2, 2, 2), (1, 3));
			CreateNormalCard(deck, 5, (3, 3, 3), (4, 4));
			CreateNormalCard(deck, 6, (4, 4, 4), (3, 3));
			CreateNormalCard(deck, 7, (5, 5, 5), (1, 4));
			CreateNormalCard(deck, 8, (6, 6, 6), (4, 1));
			CreateNormalCard(deck, 9, (7, 7, 7), (3, 2));
			CreateNormalCard(deck, 10, (8, 8, 8), (2, 3));
			CreateNormalCard(deck, 11, (9, 9, 9), (3, 4));
			CreateNormalCard(deck, 12, (10, 10, 10), (4, 3));
			presetCardsList[2] = deck;
		}
		return CloneCardArray(presetCardsList[2]);
	}
	public static Card[] PresetCardsList3() {
		if (presetCardsList[3] == null) {
			Card[] deck = new Card[CardsListManager.MAX_TOTAL_CARDS];
			CreateSpecialCard(deck, 1, CardType.Bomb, 2, 3);
			CreateSpecialCard(deck, 2, CardType.Bomb, 3, 2);
			CreateNormalCard(deck, 3, (2, 1, 3), (2, 4));
			CreateNormalCard(deck, 4, (3, 1, 5), (4, 2));
			CreateNormalCard(deck, 5, (5, 1, 9), (1, 4));
			CreateNormalCard(deck, 6, (5, 1, 9), (3, 4));
			CreateNormalCard(deck, 7, (5, 1, 9), (4, 4));
			CreateNormalCard(deck, 8, (5, 1, 9), (1, 3));
			CreateNormalCard(deck, 9, (5, 1, 9), (3, 3));
			CreateNormalCard(deck, 10, (5, 1, 9), (4, 3));
			CreateNormalCard(deck, 11, (5, 1, 9), (1, 2));
			CreateNormalCard(deck, 12, (5, 1, 9), (2, 2));
			CreateNormalCard(deck, 13, (5, 1, 9), (3, 1));
			CreateNormalCard(deck, 14, (5, 1, 9), (4, 1));
			presetCardsList[3] = deck;
		}
		return CloneCardArray(presetCardsList[3]);
	}
	public static Card[] PresetCardsList4() {
		if (presetCardsList[4] == null) {
			Card[] deck = new Card[CardsListManager.MAX_TOTAL_CARDS];
			CreateSpecialCard(deck, 1, CardType.Bomb, 1, 3);
			CreateSpecialCard(deck, 2, CardType.Bomb, 3, 1);
			CreateNormalCard(deck, 3, (1, 1, 1), (4, 4));
			CreateNormalCard(deck, 4, (3, 3, 3), (1, 4));
			CreateNormalCard(deck, 5, (3, 3, 3), (3, 4));
			CreateNormalCard(deck, 6, (3, 3, 3), (3, 2));
			CreateNormalCard(deck, 7, (5, 5, 5), (2, 3));
			CreateNormalCard(deck, 8, (5, 5, 5), (4, 3));
			CreateNormalCard(deck, 9, (5, 5, 5), (4, 1));
			CreateNormalCard(deck, 10, (10, 10, 10), (2, 4));
			CreateNormalCard(deck, 11, (10, 10, 10), (3, 3));
			CreateNormalCard(deck, 12, (10, 10, 10), (4, 2));
			presetCardsList[4] = deck;
		}
		return CloneCardArray(presetCardsList[4]);
	}






	static void CreateSpecialCard(Card[] deck, int index, CardType cardType, int positionX, int positionY) {
		deck[index-1] = new Card() {
			exists = true,
			isSelfCard = true,
			isVisible = true,
			index = index-1,
			cardType = cardType,
			positionX = positionX - 1,
			positionY = positionY - 1
		};
	}
	static void CreateNormalCard(Card[] deck, int index, (int level, int hp, int atk) data, (int positionX, int positionY) position) {
		deck[index-1] = new Card() { 
			exists = true, 
			isSelfCard = true,
			isVisible = true,
			index = index-1, 
			cardType = CardType.Normal, 
			level = data.level, 
			hp = data.hp, 
			atk = data.atk, 
			positionX = position.positionX - 1, 
			positionY = position.positionY - 1 
		};
	}






	static Card[] CloneCardArray(Card[] source) {
		if (source == null) {
			return null;
		}
		Card[] clone = new Card[source.Length];
		for (int i = 0; i < source.Length; i++) {
			clone[i] = CloneCard(source[i]);
		}
		return clone;
	}
    static Card CloneCard(Card source) {
		if (source == null) {
			return null;
		}
		return new Card() {
			exists = source.exists,
			isSelfCard = source.isSelfCard,
			isVisible = source.isVisible,
			index = source.index,
			cardType = source.cardType,
			level = source.level,
			hp = source.hp,
			atk = source.atk,
			positionX = source.positionX,
			positionY = source.positionY,
		};
	}
}
