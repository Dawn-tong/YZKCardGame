using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSettingChessInfo : MonoBehaviour {
    [Header("ø®∆¨ Ù–‘")]
    public Card card;
    public int index;
    [SerializeField] TextMesh indexText;
    
    [Header("∆’Õ®ø®≈∆")]
	[SerializeField] GameObject normalPanel;
	[SerializeField] TextMesh LevelText;
    [SerializeField] TextMesh HpText;
    [SerializeField] TextMesh AtkText;

    [Header("Ãÿ ‚ø®≈∆")]
	[SerializeField] GameObject specialPanel;
    [SerializeField] TextMesh SkillText;

    public void UpdateChessInfo() {
        if(card == null) {
			//…æ≥˝◊‘º∫
			Destroy(gameObject);
			return;
        }
        indexText.text = (index + 1).ToString();
        if (card.cardType == CardType.Normal) {
            normalPanel.SetActive(true);
            specialPanel.SetActive(false);
            LevelText.text = card.level.ToString();
            HpText.text = card.hp.ToString();
            AtkText.text = card.atk.ToString();
        } else {
            normalPanel.SetActive(false);
            specialPanel.SetActive(true);
            SkillText.text = "’®µØ";
        }
    }
}
