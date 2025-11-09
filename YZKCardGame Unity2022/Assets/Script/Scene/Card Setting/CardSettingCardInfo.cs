using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSettingCardInfo : MonoBehaviour {

	[HideInInspector] public Card card;
    [Header("±‡∫≈")]
    public Text indexText;
    [Header("∆’Õ®ø®≈∆")]
    public GameObject normalPanel;
    public Text LevelText;
    public Text HpText;
    public Text AtkText;
    [Header("Ãÿ ‚ø®≈∆")]
    public GameObject specialPanel;
    public Text SpecialText;






	void Start() {
        UpdateCardInfo();
    }
    public void UpdateCardInfo() {
        indexText.text = (card.cardIndex + 1).ToString();
        if(card == null) {
            normalPanel.SetActive(true);
            specialPanel.SetActive(false);
            LevelText.text = "--";
            HpText.text = "--";
            AtkText.text = "--";
            return;
        }
        if (card.cardType == CardType.Normal) {
            normalPanel.SetActive(true);
            specialPanel.SetActive(false);
            LevelText.text = card.level.ToString();
            HpText.text = card.hp.ToString();
            AtkText.text = card.atk.ToString();
        }
        else if (card.cardType == CardType.Bomb) {
            normalPanel.SetActive(false);
            specialPanel.SetActive(true);
            SpecialText.text = "’®µØ";
        }
    }
}
