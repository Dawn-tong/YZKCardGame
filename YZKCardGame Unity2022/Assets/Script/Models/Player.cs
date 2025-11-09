using UnityEngine;

public class Player : MonoBehaviour {
	public ulong netID;//服务器标记玩家的网络ID
	public int seatID;//用于标记自己在服务器中的座位
	public string playerName;
	public bool isReady = false;
	public CardManager cardManager;
	
	public void Init(int seatID) {
		this.seatID = seatID;
		// 初始化CardManager
		if (cardManager == null) {
			cardManager = gameObject.AddComponent<CardManager>();
		}
		cardManager.Init(this);
	}
	public Player SetNetID(ulong netID) {
		this.netID = netID;
		return this;
	}
	public Player SetSeatID(int seatID) {
		this.seatID = seatID;
		return this;
	}
	public Player SetPlayerName(string name) {
		playerName = name;
		return this;
	}
	public Player SetReady(bool ready) {
		isReady = ready;
		return this;
	}
}
