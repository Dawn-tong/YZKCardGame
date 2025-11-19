using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomSceneUI : MonoBehaviour {
	public void OnSceneEnter() {
		RoomService.Instance.UpdateUIEvent += UpdatePlayerPanelData;
		if (NetManager.Instance.isHostPlayer) {
			NetManager.Instance.OnCreatRoomSuccess += DisplayUI;
			creatOrAddRoomText.text = "正在创建房间......";
		}
		else {
			NetManager.Instance.OnJoinRoomSuccess += DisplayUI;
			creatOrAddRoomText.text = "正在加入房间......";
			roomNumberText.text = $"房间号: {NetManager.Instance.GetCurrentJoinCode()}";
		}
	}
	public void OnSceneLeave() {
		RoomService.Instance.UpdateUIEvent -= UpdatePlayerPanelData;
		if (NetManager.Instance.isHostPlayer) {
			NetManager.Instance.OnCreatRoomSuccess -= DisplayUI;
		}
		else {
			NetManager.Instance.OnJoinRoomSuccess -= DisplayUI;
		}
	}






	[SerializeField] GameObject roomNumberPanel;
	[SerializeField] Text roomNumberText;
	[SerializeField] Text creatOrAddRoomText;
	[SerializeField] GameObject allPlayerPanel;
	[SerializeField] GameObject[] playerTextPanel = new GameObject[4];
	[SerializeField] Text[] playerNameText = new Text[4];
	[SerializeField] Text[] readyStateText = new Text[4];
	[SerializeField] Button startButton;
	[SerializeField] Button readyButton;
	[SerializeField] Text readyButtonText;
	[SerializeField] Button leaveRoomButton;
	public void ClickButtonToCopyRoomNumber() {
		//复制房间号到剪贴板
		GUIUtility.systemCopyBuffer = NetManager.Instance.GetCurrentJoinCode();
		UIMessagePanel.Instance.AddMessage("房间号已复制到剪贴板");
	}
	public void ClickButtonToStartGame() {
		Debug.Log("按钮 - 开始游戏");
		SceneLoaderManager.Instance.LoadScene(Scene.GameScene);
		RoomService.Instance.SendReadyToStartResponse();
	}
	public void ClickButtonToChangeReady()
	{
		if (PlayerManager.Instance.currentPlayer.isReady) {
			PlayerManager.Instance.currentPlayer.isReady = false;
			readyButtonText.text = "准备";
		}
		else {
			PlayerManager.Instance.currentPlayer.isReady = true;
			readyButtonText.text = "取消准备";
		}
		UpdatePlayerPanelData();
		if (NetManager.Instance.isHostPlayer) {
			RoomService.Instance.SendSelfChangeReadyResponse();
		}
		else {
			RoomService.Instance.SendSelfChangeReadyRequest();
		}
	}
	public void ClickButtonToLeaveRoom() {
		Debug.Log("按钮 - 离开房间");
		if(NetManager.Instance.isHostPlayer) {
			RoomService.Instance.SendRoomCloseResponse();
		}
		else {
			RoomService.Instance.SendLeaveRoomRequest();
		}
	}






	void DisplayUI() {
		//隐藏加载UI
		creatOrAddRoomText.gameObject.SetActive(false);
		//显示房间号
		roomNumberText.text = $"房间号: {NetManager.Instance.GetCurrentJoinCode()}";
		roomNumberPanel.SetActive(true);
		//显示所有玩家
		allPlayerPanel.gameObject.SetActive(true);
		//显示准备按钮
		readyButton.gameObject.SetActive(true);
		//显示离开房间
		leaveRoomButton.gameObject.SetActive(true);
		//根据是否存在玩家刷新显示
		UpdatePlayerPanelData();
	}
	void UpdatePlayerPanelData() {

		int countPlayer = 0;	//总人数
		bool allReady = true;	//所有玩家均准备
		
		for (int i = 0; i < PlayerManager.MAX_PLAYER_NUM; i++) {
			Player player = PlayerManager.Instance.allPlayers[i];
			if (player != null) {
				//总人数
				countPlayer++;
				//设置玩家名字
				string playerName = player.playerName;
				if (player == PlayerManager.Instance.currentPlayer) {
					playerName += "\n(自己)";
				}
				playerNameText[i].text = playerName;
				//设置玩家准备状态
				if (player.isReady) {
					readyStateText[i].text = "√";
				}
				else {
					readyStateText[i].text = "未准备";
					allReady = false;
				}
				
				playerTextPanel[i].SetActive(true);
			}
			else {
				playerTextPanel[i].SetActive(false);
			}
		}
		//服务器发现所有玩家均准备
		if (NetManager.Instance.isHostPlayer && allReady && countPlayer >= 2) {
			startButton.gameObject.SetActive(true);
		}
		else {
			startButton.gameObject.SetActive(false);
		}
	}
}