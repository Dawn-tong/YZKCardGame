using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess : MonoBehaviour {
	[Header("ø®∆¨ Ù–‘")]
	public Card card;
	[SerializeField] SpriteRenderer bgSpriteRenderer;
	[SerializeField] GameObject indexPanel;
	[SerializeField] TextMesh indexText;
	
	[Header("∆’Õ®ø®≈∆")]
	[SerializeField] GameObject normalPanel;
	[SerializeField] GameObject levelPanel;
	[SerializeField] TextMesh LevelText;
	[SerializeField] TextMesh HpText;
	[SerializeField] TextMesh AtkText;
	
	[Header("Ãÿ ‚ø®≈∆")]
	[SerializeField] GameObject specialPanel;
	[SerializeField] TextMesh SkillText;

	public void UpdateChess() {
		if(!card.isVisible) {
			normalPanel.SetActive(false);
			specialPanel.SetActive(false);
			return;
		}
		if (card.cardType == CardType.Normal) {
			normalPanel.SetActive(true);
			specialPanel.SetActive(false);
			LevelText.text = card.level.ToString();
			HpText.text = card.hp.ToString();
			AtkText.text = card.atk.ToString();
		} 
		else {
			normalPanel.SetActive(false);
			specialPanel.SetActive(true);
			SkillText.text = "’®µØ";
		}
	}

	public void ShowInfo() {
		card.isVisible = true;
		UpdateChess();
	}

	public void ShowIndex() {
		levelPanel.transform.position = new Vector3(0, 0, 0);
		indexPanel.SetActive(true);
		indexText.text = (card.index + 1).ToString();
	}

	public void SetColor(Color color) {
		bgSpriteRenderer.color = color;
	}

	//œ˙ªŸ
	public void DestroyChess() {
		Destroy(gameObject);
		card.chessComponent = null;
	}
}
