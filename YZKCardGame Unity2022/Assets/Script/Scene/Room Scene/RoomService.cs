using Network;
using ProtoMessage;
using UnityEngine;

public class RoomService
{
	static RoomService instance;
	public static RoomService Instance {
		get {
			if (instance == null)
				instance = new RoomService();
			return instance;
		}
	}






	//进入场景时
	public void OnSceneEnter() {
		if (NetManager.Instance.isHostPlayer) {
			MessageDistributer<ulong>.Instance.Subscribe<PlayerJoinRoomRequest>(OnPlayerJoinRoomRequest);
			MessageDistributer<ulong>.Instance.Subscribe<ChangeReadyRequest>(OnPlayerChangeReadyRequest);
			NetManager.Instance.NetDisconnected += OnNetDisconnected;
			// 主机玩家创建房间
			NetManager.Instance.CreateRelayRoom(4);
		}
		else {
			MessageDistributer<ulong>.Instance.Subscribe<PlayerJoinRoomResponse>(OnPlayerJoinRoomResponse);
			MessageDistributer<ulong>.Instance.Subscribe<AddPlayerToRoomResponse>(OnAddPlayerToRoomResponse);
			MessageDistributer<ulong>.Instance.Subscribe<LeaveRoomResponse>(OnLeaveRoomResponse);
			MessageDistributer<ulong>.Instance.Subscribe<ChangeReadyResponse>(OnPlayerChangeReadyResponse);
			//MessageDistributer<ulong>.Instance.Subscribe<RoomCloseResponse>(OnRoomCloseResponse);
			NetManager.Instance.NetDisconnected += OnRoomCloseResponse;
			NetManager.Instance.OnJoinRoomSuccess += SendPlayerJoinRoomRequest;
			NetManager.Instance.OnJoinRoomFailed += OnJoinRoomFailed;
			if (string.IsNullOrEmpty(NetManager.Instance.GetCurrentJoinCode())) {
				SceneLoaderManager.Instance.LoadScene(Scene.HallScene);
			}
		}
	}
	//离开场景时
	public void OnSceneLeave() {
		if (NetManager.Instance.isHostPlayer) {
			MessageDistributer<ulong>.Instance.Unsubscribe<PlayerJoinRoomRequest>(OnPlayerJoinRoomRequest);
			MessageDistributer<ulong>.Instance.Unsubscribe<ChangeReadyRequest>(OnPlayerChangeReadyRequest);
			NetManager.Instance.NetDisconnected -= OnNetDisconnected;
		}
		else {
			MessageDistributer<ulong>.Instance.Unsubscribe<PlayerJoinRoomResponse>(OnPlayerJoinRoomResponse);
			MessageDistributer<ulong>.Instance.Unsubscribe<AddPlayerToRoomResponse>(OnAddPlayerToRoomResponse);
			MessageDistributer<ulong>.Instance.Unsubscribe<LeaveRoomResponse>(OnLeaveRoomResponse);
			MessageDistributer<ulong>.Instance.Unsubscribe<ChangeReadyResponse>(OnPlayerChangeReadyResponse);
			//MessageDistributer<ulong>.Instance.Unsubscribe<RoomCloseResponse>(OnRoomCloseResponse);
			NetManager.Instance.NetDisconnected -= OnRoomCloseResponse;
			NetManager.Instance.OnJoinRoomSuccess -= SendPlayerJoinRoomRequest;
			NetManager.Instance.OnJoinRoomFailed -= OnJoinRoomFailed;
		}
	}
	//加入失败时
	void OnJoinRoomFailed() {
		SceneLoaderManager.Instance.LoadScene(Scene.HallScene);
	}
	//离开房间时
	void OnLeaveRoom() {
		NetManager.Instance.LeaveRoom();
		PlayerManager.Instance.ClearAllPlayersExpectSelf();
		PlayerManager.Instance.MoveCurrentPlayerBaseSeatID(0);
		PlayerManager.Instance.currentPlayer.SetReady(false);
		SceneLoaderManager.Instance.LoadScene(Scene.HallScene);
	}






	//服务器: 发送自己准备状态修改的响应(点击按钮触发)
	public void SendSelfChangeReadyResponse() {
		NetMessage message = new NetMessage();
		message.Response = new NetMessageResponse();
		message.Response.changeReady = new ChangeReadyResponse();
		message.Response.changeReady.seatID = PlayerManager.Instance.currentPlayer.seatID;
		message.Response.changeReady.readyState = PlayerManager.Instance.currentPlayer.isReady;
		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        通知所有玩家        ————");
		Debug.Log($"{Log.perfix}消息内容:自己改变了准备状态为{PlayerManager.Instance.currentPlayer.isReady}");
		UIMessagePanel.Instance.AddMessage($"通知所有玩家:自己改变了准备状态为{PlayerManager.Instance.currentPlayer.isReady}");
		NetManager.Instance.SendMessageToAll(message);
		Log.ReducePerfixLength();
	}
	//服务器:发送房间销毁响应(点击按钮触发)
	public void SendRoomCloseResponse() {
		//NetMessage message = new NetMessage();
		//message.Response = new NetMessageResponse();
		//message.Response.roomClose = new RoomCloseResponse();

		//Log.IncreasePerfixLength();
		//Debug.Log($"{Log.perfix}————        通知所有玩家        ————");
		//Debug.Log($"{Log.perfix}消息内容:房间关闭通知");
		//UIMessagePanel.Instance.AddMessage($"通知所有玩家，房间关闭。");
		//NetManager.Instance.SendMessageToAll(message);
		//Log.ReducePerfixLength();

		OnLeaveRoom();
	}
	//客户端:发送玩家进入请求(通过事件触发)
	void SendPlayerJoinRoomRequest() {
		NetMessage message = new NetMessage();
		message.Request = new NetMessageRequest();
		message.Request.playerJoinRoom = new PlayerJoinRoomRequest();
		message.Request.playerJoinRoom.playerName = PlayerManager.Instance.currentPlayer.playerName;

		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        发送消息给服务器        ————");
		Debug.Log($"{Log.perfix}消息内容:申请进入房间");
		UIMessagePanel.Instance.AddMessage($"通知服务器:申请进入房间");
		NetManager.Instance.SendMessageToServer(message);
		Log.ReducePerfixLength();
	}
	//客户端: 发送玩家改变准备状态请求(点击按钮触发)
	public void SendSelfChangeReadyRequest() {
		NetMessage message = new NetMessage();
		message.Request = new NetMessageRequest();
		message.Request.changeReady = new ChangeReadyRequest();
		message.Request.changeReady.readyState = PlayerManager.Instance.currentPlayer.isReady;

		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        发送消息给服务器        ————");
		Debug.Log($"{Log.perfix}消息内容:自己改变了准备状态为{PlayerManager.Instance.currentPlayer.isReady}");
		UIMessagePanel.Instance.AddMessage($"通知服务器:自己改变了准备状态为{PlayerManager.Instance.currentPlayer.isReady}");
		NetManager.Instance.SendMessageToServer(message);
		Log.ReducePerfixLength();
	}
	//客户端:发送离开房间请求(点击按钮触发)
	public void SendLeaveRoomRequest() {
		//NetMessage message = new NetMessage();
		//message.Request = new NetMessageRequest();
		//message.Request.leaveRoom = new LeaveRoomRequest();
		//message.Request.leaveRoom.selfSeatID = PlayerManager.Instance.currentPlayer.seatID;

		//Log.IncreasePerfixLength();
		//Debug.Log($"{Log.perfix}————        发送消息给服务器        ————");
		//Debug.Log($"{Log.perfix}消息内容:自己离开了房间");
		//UIMessagePanel.Instance.AddMessage($"通知服务器:自己离开了房间");
		//NetManager.Instance.SendMessageToServer(message);
		//Log.ReducePerfixLength();
		OnLeaveRoom();
	}






	public delegate void UpdateUIDelegate();
	public event UpdateUIDelegate UpdateUIEvent;
	/// <summary>
	/// 服务器端处理玩家加入房间的请求
	/// </summary>
	void OnPlayerJoinRoomRequest(ulong senderId, PlayerJoinRoomRequest request) {
		Debug.Log($"{Log.perfix}消息内容:玩家(Name={request.playerName})申请加入房间");
		UIMessagePanel.Instance.AddMessage($"接收:玩家(Name={request.playerName})申请加入房间");
		// 检查是否有空位置
		(bool success, Player newPlayer) = PlayerManager.Instance.CreatPlayerAtFirstAvailableSeat();
		if (!success) {
			NetMessage failureMessage = new NetMessage();
			failureMessage.Response = new NetMessageResponse();
			failureMessage.Response.PlayerJoinRoom = new PlayerJoinRoomResponse();
			failureMessage.Response.PlayerJoinRoom.successJoin = false;
			Log.IncreasePerfixLength();
			Debug.Log($"{Log.perfix}————        发送消息给玩家{senderId}        ————");
			Debug.Log($"{Log.perfix}消息内容:房间进入失败");
			UIMessagePanel.Instance.AddMessage($"通知玩家{senderId}:房间进入失败");
			NetManager.Instance.SendMessageToPlayer(senderId, failureMessage);
			Log.ReducePerfixLength();
			return;
		}
		newPlayer.SetNetID(senderId).SetPlayerName(request.playerName);
		//更新服务器自己的UI
		UpdateUIEvent?.Invoke();

		//向新加入的玩家发送完整的玩家列表（包括他自己）
		NetMessage message = new NetMessage();
		message.Response = new NetMessageResponse();
		message.Response.PlayerJoinRoom = new PlayerJoinRoomResponse();
		message.Response.PlayerJoinRoom.successJoin = true;
		message.Response.PlayerJoinRoom.selfSeatID = newPlayer.seatID;
		message.Response.PlayerJoinRoom.selfNetID = senderId;
		for (int i = 0; i < PlayerManager.Instance.allPlayers.Length; i++) {
			Player player = PlayerManager.Instance.allPlayers[i];
			if (player != null) {
				var playerInfo = new PlayerInfo {
					seatID = i,
					playerName = player.playerName,
					isReady = player.isReady
				};
				message.Response.PlayerJoinRoom.allPlayers.Add(playerInfo);
			}
		}
		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        发送消息给玩家{senderId}        ————");
		Debug.Log($"{Log.perfix}消息内容: 进入成功，并告知所有玩家名单。");
		UIMessagePanel.Instance.AddMessage($"通知玩家{senderId}:房间进入成功，并告知所有玩家名单");
		NetManager.Instance.SendMessageToPlayer(senderId, message);
		Log.ReducePerfixLength();

		// 4. 通知其他已在房间的玩家，有新玩家加入（只发送新玩家的信息）
		NetMessage notifyMessage = new NetMessage();
		notifyMessage.Response = new NetMessageResponse();
		notifyMessage.Response.AddPlayerToRoom = new AddPlayerToRoomResponse();
		notifyMessage.Response.AddPlayerToRoom.player = new PlayerInfo {
			seatID = newPlayer.seatID,
			playerName = newPlayer.playerName,
			isReady = newPlayer.isReady
		};
		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        通知除了发送者(NetID={senderId})的其余玩家        ————");
		Debug.Log($"{Log.perfix}消息内容:NetID={senderId}的玩家进入了房间");
		UIMessagePanel.Instance.AddMessage($"通知其余玩家，NetID={senderId}的玩家进入了房间");
		NetManager.Instance.SendMessageToAll(notifyMessage, senderId);
		Log.ReducePerfixLength();
	}
	/// <summary>
	/// 服务器处理玩家改变准备状态的请求
	/// </summary>
	void OnPlayerChangeReadyRequest(ulong senderId, ChangeReadyRequest request) {
		Debug.Log($"{Log.perfix}消息内容:玩家(NetID={senderId})设置准备状态为{request.readyState}");
		UIMessagePanel.Instance.AddMessage($"接收:玩家(NetID={senderId})设置准备状态为{request.readyState}");
		Player player = PlayerManager.Instance.FindPlayerByNetID(senderId);
		if (player != null) {
			player.SetReady(request.readyState);
			UpdateUIEvent?.Invoke();
		}
		NetMessage message = new NetMessage();
		message.Response = new NetMessageResponse();
		message.Response.changeReady = new ChangeReadyResponse();
		message.Response.changeReady.seatID = player.seatID;
		message.Response.changeReady.readyState = request.readyState;
		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        通知除了发送者(NetID={senderId})的其余玩家        ————");
		Debug.Log($"{Log.perfix}消息内容:玩家(座位={player.seatID})的准备状态为{request.readyState}");
		UIMessagePanel.Instance.AddMessage($"通知其余玩家，玩家(座位={player.seatID})的准备状态为{request.readyState}");
		NetManager.Instance.SendMessageToAll(message, senderId);
		Log.ReducePerfixLength();
	}
	/// <summary>
	/// 服务器接收玩家掉线(通过Relay断连回调触发)
	/// </summary>
	void OnNetDisconnected(ulong clientID) {
		int seatID = PlayerManager.Instance.FindPlayerByNetID(clientID).seatID;
		Debug.Log($"{Log.perfix}OnNetDisconnected: 玩家(座位={seatID})离开房间");
		UIMessagePanel.Instance.AddMessage($"玩家(座位={seatID})离开房间");
		PlayerManager.Instance.RemovePlayerBySeatID(seatID);

		NetMessage message = new NetMessage();
		message.Response = new NetMessageResponse();
		message.Response.leaveRoom = new LeaveRoomResponse();
		message.Response.leaveRoom.triggerSeatID = seatID;

		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        通知所有玩家        ————");
		Debug.Log($"{Log.perfix}消息内容: 玩家(座位={seatID})离开了房间");
		UIMessagePanel.Instance.AddMessage($"通知所有玩家，玩家(座位={seatID})离开了房间");
		NetManager.Instance.SendMessageToAll(message);
		Log.ReducePerfixLength();

		// 更新UI显示
		UpdateUIEvent?.Invoke();
	}

	/// <summary>
	/// 客户端接收服务器的玩家加入房间响应（新玩家收到完整玩家列表）
	/// </summary>
	void OnPlayerJoinRoomResponse(ulong senderId, PlayerJoinRoomResponse response) {
		Debug.Log($"{Log.perfix}消息内容:接收所有玩家列表，玩家数量 = {response.allPlayers.Count}"); 
		UIMessagePanel.Instance.AddMessage($"接收:成功进入房间");
		UIMessagePanel.Instance.AddMessage($"接收:所有玩家列表");
		PlayerManager.Instance.MoveCurrentPlayerBaseSeatID(response.selfSeatID);
		PlayerManager.Instance.currentPlayer.SetNetID(response.selfNetID);
		PlayerManager.Instance.ClearAllPlayersExpectSelf();
		foreach (PlayerInfo playerInfo in response.allPlayers) {
			if (playerInfo.seatID == response.selfSeatID) {
				//不创建自身玩家
				break;
			}
			//根据座位ID创建玩家到数组位置
			(bool success, Player newPlayer) = PlayerManager.Instance.CreatePlayerAtSpecificSeat(playerInfo.seatID);
			if (!success) {
				Debug.LogWarning($"创建玩家失败，座位ID = {playerInfo.seatID}");
				UIMessagePanel.Instance.AddMessage($"错误:创建玩家失败，座位ID = {playerInfo.seatID}");
				continue;
			}
			newPlayer.SetPlayerName(playerInfo.playerName).SetReady(playerInfo.isReady)	;
		}
		// 更新UI显示
		UpdateUIEvent?.Invoke();
	}
	/// <summary>
	/// 客户端接收新玩家加入房间的通知（原有玩家收到单个新玩家信息）
	/// </summary>
	void OnAddPlayerToRoomResponse(ulong senderId, AddPlayerToRoomResponse response) {
		Debug.Log($"{Log.perfix}消息内容:玩家(座位={response.player.seatID})加入房间，玩家名称={response.player.playerName}");
		UIMessagePanel.Instance.AddMessage($"接收:玩家(座位={response.player.seatID})加入房间，玩家名称={response.player.playerName}");
		(bool success, Player newPlayer) = PlayerManager.Instance.CreatePlayerAtSpecificSeat(response.player.seatID);
		if (!success) {
			Debug.LogWarning($"创建玩家失败，座位ID = {response.player.seatID}");
			UIMessagePanel.Instance.AddMessage($"错误:创建玩家失败，座位ID = {response.player.seatID}");
			return;
		}
		newPlayer.SetPlayerName(response.player.playerName).SetReady(response.player.isReady);
		// 更新UI显示
		UpdateUIEvent?.Invoke();
	}
	/// <summary>
	/// 客户端处理玩家改变准备状态的响应
	/// </summary>
	void OnPlayerChangeReadyResponse(ulong senderId, ChangeReadyResponse response) {
		Debug.Log($"{Log.perfix}消息内容:玩家(座位={response.seatID})的准备状态为{response.readyState}");
		UIMessagePanel.Instance.AddMessage($"接收:玩家(座位={response.seatID})的准备状态为{response.readyState}");
		Player player = PlayerManager.Instance.FindPlayerBySeatID(response.seatID);
		if (player != null) {
			player.SetReady(response.readyState);
			UpdateUIEvent?.Invoke();
		}
	}
	/// <summary>
	/// 客户端收到其他玩家离开的通知
	/// </summary>
	void OnLeaveRoomResponse(ulong arg1, LeaveRoomResponse response) {
		Debug.Log($"{Log.perfix}消息内容:玩家(座位={response.triggerSeatID})离开房间");
		UIMessagePanel.Instance.AddMessage($"接收:玩家(座位={response.triggerSeatID})离开房间");
		PlayerManager.Instance.RemovePlayerBySeatID(response.triggerSeatID);
		// 更新UI显示
		UpdateUIEvent?.Invoke();
	}
	/// <summary>
	/// 客户端接收服务器房间关闭(通过Relay断连回调触发)
	/// </summary>
	void OnRoomCloseResponse(ulong clientID) {
		OnLeaveRoom();
	}
}
