using UnityEngine;

public class PlayerManager : ManagerBase<PlayerManager> {

	public const int MAX_PLAYER_NUM = 4;

	public Player currentPlayer;
	public GameObject[] playerObjects = new GameObject[MAX_PLAYER_NUM];
	public Player[] allPlayers = new Player[MAX_PLAYER_NUM];
	public void Init() {
		//创建主机玩家
		(_, currentPlayer) = CreatPlayerAtFirstAvailableSeat();
		currentPlayer.SetPlayerName("主机玩家");
		currentPlayer.gameObject.name = "Current Player";
		//加载玩家卡组
		CardsStorageManager.Init();
		GameManager.FinishInit();
	}





	//创建
	public (bool success, Player player) CreatPlayerAtFirstAvailableSeat() {
		for (int i = 0; i < allPlayers.Length; i++) {
			if (allPlayers[i] == null) {
				Player player = CreatPlayerBySeatID(i);
				return (true, player);
			}
		}
		Debug.LogWarning("玩家数组已满，无法创建新玩家");
		return (false, null);
	}

	public (bool success, Player player) CreatePlayerAtSpecificSeat(int seatID) {
		if (seatID < 0 || seatID >= 4) {
			Debug.LogWarning("座位ID无效，无法创建新玩家");
			return (false, null);
		}
		if (allPlayers[seatID] != null) {
			Debug.LogWarning("座位ID已存在，无法创建新玩家");
			return (false, null);
		}
		Player player = CreatPlayerBySeatID(seatID);
		return (true, player);
	}

	Player CreatPlayerBySeatID(int seatID) {
		GameObject playerObj = new GameObject($"Player_{seatID}");
		DontDestroyOnLoad(playerObj);
		playerObj.transform.SetParent(ManagerObj.transform);
		// 添加Player组件并初始化
		Player player = playerObj.AddComponent<Player>();
		player.Init(seatID);
		// 存储到数组指定位置
		playerObjects[seatID] = playerObj;
		allPlayers[seatID] = player;
		return player;
	}

	//查找
	public Player FindPlayerBySeatID(int seatID) {
		if (seatID >= 0 && seatID < 4) {
			if (allPlayers[seatID] != null) {
				return allPlayers[seatID];
			}
		}
		return null;
	}

	public Player FindPlayerByNetID(ulong netID) {
		for (int i = 0; i < allPlayers.Length; i++) {
			if (allPlayers[i] != null && allPlayers[i].netID == netID) {
				return allPlayers[i];
			}
		}
		return null;
	}

	//删除
	public void RemovePlayerBySeatID(int seatID) {
		if (seatID >= 0 && seatID < 4) {
			if (allPlayers[seatID] != null) {
				Destroy(playerObjects[seatID]);
				playerObjects[seatID] = null;
				allPlayers[seatID] = null;
			}
		}
	}

	public void ClearAllPlayersExpectSelf() {
		for (int i = 0; i < allPlayers.Length; i++) {
			if (allPlayers[i] != null && i != currentPlayer.seatID) {
				Destroy(playerObjects[i]);
				playerObjects[i] = null;
				allPlayers[i] = null;
			}
		}
	}

	//移动自身到某位置
	public void MoveCurrentPlayerBaseSeatID(int seatID) {
		if(seatID < 0 || seatID >= 4) {
			Debug.LogWarning("座位ID无效，无法设置当前玩家");
			return;
		}
		//交换座位
		int originalSeat = currentPlayer.seatID;
		(playerObjects[originalSeat], playerObjects[seatID]) = (playerObjects[seatID], playerObjects[originalSeat]);
		(allPlayers[originalSeat], allPlayers[seatID]) = (allPlayers[seatID], allPlayers[originalSeat]);
		//更新座位
		allPlayers[originalSeat]?.SetSeatID(originalSeat);
		allPlayers[seatID].SetSeatID(seatID);
		//重置当前玩家
		currentPlayer = allPlayers[seatID];

	}






	//获取玩家数量
	public int GetPlayerCount() {
		int count = 0;
		for (int i = 0; i < allPlayers.Length; i++) {
			if (allPlayers[i] != null) {
				count++;
			}
		}
		return count;
	}
}