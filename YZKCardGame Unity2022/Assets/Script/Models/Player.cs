using UnityEngine;

public enum CornerID {
	None,
	BottomLeft,
	TopLeft,
	TopRight,
	BottomRight,
}

public class Player : MonoBehaviour {
	public ulong netID;	//服务器标记玩家的网络ID
	public int seatID;	//用于标记自己在服务器中的座位
	public CornerID cornerID;	//用于标记自己在棋盘中的角落
	public string playerName;
	public bool isReady = false;
	public CardsListManager cardManager;	//当前的卡组
	public CardsListManager gameCardManager;	//游戏中的临时卡组
	
	public void Init(int seatID) {
		this.seatID = seatID;
		this.cornerID = CornerID.BottomLeft;
		// 初始化CardManager
		cardManager = gameObject.AddComponent<CardsListManager>();
		cardManager.Init(this);
		gameCardManager = gameObject.AddComponent<CardsListManager>();
		gameCardManager.Init(this);
	}
	public Player SetNetID(ulong netID) {
		this.netID = netID;
		return this;
	}
	public Player SetSeatID(int seatID) {
		this.seatID = seatID;
		return this;
	}
	public Player SetCornerID(CornerID cornerID) {
		this.cornerID = cornerID;
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
