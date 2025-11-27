using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUI : MonoBehaviour {
	static GameSceneUI instance;
	public static GameSceneUI Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<GameSceneUI>();
			}
			return instance;
		}
	}

	[SerializeField] GameObject BottomLeftPanel;
	[SerializeField] GameObject TopLeftPanel;
	[SerializeField] GameObject TopRightPanel;
	[SerializeField] GameObject BottomRightPanel;
	[SerializeField] Text BottomLeftPlayerNameText;
	[SerializeField] Text TopLeftPlayerNameText;
	[SerializeField] Text TopRightPlayerNameText;
	[SerializeField] Text BottomRightPlayerNameText;
	[SerializeField] GameObject BottomLeftCurrentTurnText;
	[SerializeField] GameObject TopLeftCurrentTurnText;
	[SerializeField] GameObject TopRightCurrentTurnText;
	[SerializeField] GameObject BottomRightCurrentTurnText;

	public void UpdateUI() {
		BottomLeftPanel.SetActive(false);
		TopLeftPanel.SetActive(false);
		TopRightPanel.SetActive(false);
		BottomRightPanel.SetActive(false);
		foreach (var player in PlayerManager.Instance.allPlayers) {
			if (player == null) {
				continue;
			}
			if (player.cornerID == CornerID.BottomLeft) {
				BottomLeftPanel.SetActive(true);
				BottomLeftPlayerNameText.text = player.playerName;
			}
			else if (player.cornerID == CornerID.TopLeft) {
				TopLeftPanel.SetActive(true);
				TopLeftPlayerNameText.text = player.playerName;
			}
			else if (player.cornerID == CornerID.TopRight) {
				TopRightPanel.SetActive(true);
				TopRightPlayerNameText.text = player.playerName;
			}
			else if (player.cornerID == CornerID.BottomRight) {
				BottomRightPanel.SetActive(true);
				BottomRightPlayerNameText.text = player.playerName;
			}
		}
	}

	public void UpdateCurrentTurnText(CornerID cornerID) {
		BottomLeftCurrentTurnText.SetActive(cornerID == CornerID.BottomLeft);
		TopLeftCurrentTurnText.SetActive(cornerID == CornerID.TopLeft);
		TopRightCurrentTurnText.SetActive(cornerID == CornerID.TopRight);
		BottomRightCurrentTurnText.SetActive(cornerID == CornerID.BottomRight);
	}
}
