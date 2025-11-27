using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataStorage {
	public static string playerNameKey = "playerName";
	public static void Init() {
		LoadPlayerData();
	}

	public static void SavePlayerData() {
		PlayerPrefs.SetString(playerNameKey, PlayerManager.Instance.currentPlayer.playerName);
	}

	public static void LoadPlayerData() {
		PlayerManager.Instance.currentPlayer.SetPlayerName(PlayerPrefs.GetString(playerNameKey, "Î´ÃüÃûÍæ¼Ò"));
	}
}
