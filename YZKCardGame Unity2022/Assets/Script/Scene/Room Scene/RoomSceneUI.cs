using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomSceneUI : MonoBehaviour {
	public void OnSceneEnter() {
		RoomSceneService.Instance.UpdateUIEvent += UpdatePlayerPanelData;
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
		RoomSceneService.Instance.UpdateUIEvent -= UpdatePlayerPanelData;
		if (NetManager.Instance.isHostPlayer) {
			NetManager.Instance.OnCreatRoomSuccess -= DisplayUI;
		}
		else {
			NetManager.Instance.OnJoinRoomSuccess -= DisplayUI;
		}
	}






	public Text roomNumberText;
	public Text creatOrAddRoomText;
	public GameObject allPlayerPanel;
	public GameObject[] playerTextPanel = new GameObject[4];
	public Text[] playerNameText = new Text[4];
	public Text[] readyStateText = new Text[4];
	public Button readyButton;
	public Text readyButtonText;
	public Button leaveRoomButton;
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
			RoomSceneService.Instance.SendSelfChangeReadyResponse();
		}
		else {
			RoomSceneService.Instance.SendSelfChangeReadyRequest();
		}
	}
	public void ClickButtonToLeaveRoom() {
		Debug.Log("按钮 - 离开房间");
		if(NetManager.Instance.isHostPlayer) {
			RoomSceneService.Instance.SendRoomCloseResponse();
		}
		else {
			RoomSceneService.Instance.SendLeaveRoomRequest();
		}
	}






	void DisplayUI() {
		//隐藏加载UI
		creatOrAddRoomText.gameObject.SetActive(false);
		//显示房间号
		roomNumberText.text = $"房间号: {NetManager.Instance.GetCurrentJoinCode()}";
		roomNumberText.gameObject.SetActive(true);
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
		for (int i = 0; i < PlayerManager.Instance.allPlayers.Length; i++) {
			Player player = PlayerManager.Instance.allPlayers[i];
			if (player != null) {
				playerTextPanel[i].SetActive(true);
				string playerName = player.playerName;
				if (player == PlayerManager.Instance.currentPlayer) {
					playerName += "\n(自己)";
				}
				playerNameText[i].text = playerName;
				readyStateText[i].text = player.isReady ? "√" : "未准备";
			}
			else {
				playerTextPanel[i].SetActive(false);
			}
		}
	}
}