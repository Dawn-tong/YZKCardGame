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
        Card[] deck = new Card[CardManager.MAX_TOTAL_CARDS];
        deck[0] = new Card() {exists = true,index = 0, cardType = CardType.Bomb,positionX = 0, positionY = 0};
        deck[1] = new Card() {exists = true,index = 1, cardType = CardType.Bomb,positionX = 1, positionY = 1};
        return deck;
    }
    public static Card[] PresetCardsList1() {
        Card[] deck = new Card[CardManager.MAX_TOTAL_CARDS];
        deck[0] = new Card() {
            exists = true,
            index = 0, cardType = CardType.Bomb, level = 0,
            hp = 0,
            atk = 0,
        };
        return deck;
    }
    public static Card[] PresetCardsList2() {
        Card[] deck = new Card[CardManager.MAX_TOTAL_CARDS];
        deck[0] = new Card() {
            exists = true,
            index = 0, cardType = CardType.Bomb, level = 0,
            hp = 0,
            atk = 0,
        };
        return deck;
    }
    public static Card[] PresetCardsList3() {
        Card[] deck = new Card[CardManager.MAX_TOTAL_CARDS];
        deck[0] = new Card() {
            exists = true,
            index = 0, cardType = CardType.Bomb, level = 0,
            hp = 0,
            atk = 0,
        };
        return deck;
    }
    public static Card[] PresetCardsList4() {
        Card[] deck = new Card[CardManager.MAX_TOTAL_CARDS];
        deck[0] = new Card() {
            exists = true,
            index = 0, cardType = CardType.Bomb, level = 0,
            hp = 0,
            atk = 0,
        };
        return deck;
    }
}
