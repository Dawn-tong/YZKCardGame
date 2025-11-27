using Network;
using ProtoMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCommunicateService {
	static GameCommunicateService instance;
	public static GameCommunicateService Instance {
		get {
			if (instance == null)
				instance = new GameCommunicateService();
			return instance;
		}
	}






    //初始化
    public void BeforeStartGame() {
        if (NetManager.Instance.isHostPlayer) {
			MessageDistributer<ulong>.Instance.Subscribe<TurnActionRequest>(OnTurnActionRequest);
		}
		else {
			MessageDistributer<ulong>.Instance.Subscribe<TurnToPlayResponse>(OnTurnToPlayResponse);
			MessageDistributer<ulong>.Instance.Subscribe<PlayerActionResponse>(OnPlayerActionResponse);
		}
    }

	//结束游戏
	public void AfterEndGame() {
		if (NetManager.Instance.isHostPlayer) {
			MessageDistributer<ulong>.Instance.Unsubscribe<TurnActionRequest>(OnTurnActionRequest);
		}
		else {
			MessageDistributer<ulong>.Instance.Unsubscribe<TurnToPlayResponse>(OnTurnToPlayResponse);
			MessageDistributer<ulong>.Instance.Unsubscribe<PlayerActionResponse>(OnPlayerActionResponse);
		}
	}





	
    //服务器通知轮到哪个玩家出牌
    public void SendTurnToPlayResponse(Player player) {
		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        通知所有玩家        ————");
		Debug.Log($"{Log.perfix}消息内容:轮到seatID={player.seatID}的玩家出牌");
		UIMessagePanel.Instance.AddMessage($"通知所有玩家:轮到seatID={player.seatID}的玩家出牌");
		NetMessage message = new NetMessage();
		message.Response = new NetMessageResponse();
		message.Response.turnToPlay = new TurnToPlayResponse();
		message.Response.turnToPlay.seatID = player.seatID;
		NetManager.Instance.SendMessageToAll(message);
		Log.ReducePerfixLength();
    }

	//发送回合动作
	public void SendTurnAction(int oldX, int oldY, int newX, int newY) {
		//发送消息
		if (NetManager.Instance.isHostPlayer) {
			NetMessage message = new NetMessage();
			message.Response = new NetMessageResponse();
			message.Response.playerAction = new PlayerActionResponse();
			message.Response.playerAction.oldX = oldX;
			message.Response.playerAction.oldY = oldY;
			message.Response.playerAction.newX = newX;
			message.Response.playerAction.newY = newY;
			NetManager.Instance.SendMessageToAll(message);
			//通知下一个玩家出牌
			GameService.Instance.TurnToNextPlayer();
		}
		else {
			NetMessage message = new NetMessage();
			message.Request = new NetMessageRequest();
			message.Request.turnAction = new TurnActionRequest();
			message.Request.turnAction.oldX = oldX;
			message.Request.turnAction.oldY = oldY;
			message.Request.turnAction.newX = newX;
			message.Request.turnAction.newY = newY;
			NetManager.Instance.SendMessageToServer(message);
		}
	}






	//服务器接收回合结束
	void OnTurnActionRequest(ulong senderID, TurnActionRequest response) {
		//使自己运行这个动作
		GameSceneBoardManager.Instance.HandleTurnAction(response.oldX, response.oldY, response.newX, response.newY);
		//通知其他玩家这个动作(仅通知动作)
		Log.IncreasePerfixLength();
		Debug.Log($"{Log.perfix}————        通知其他玩家        ————");
		Debug.Log($"{Log.perfix}消息内容:seatID={senderID}的玩家运行了动作");
		UIMessagePanel.Instance.AddMessage($"通知其他玩家:seatID={senderID}的玩家运行了动作");
		NetMessage message = new NetMessage();
		message.Response = new NetMessageResponse();
		message.Response.playerAction = new PlayerActionResponse();
		message.Response.playerAction.oldX = response.oldX;
		message.Response.playerAction.oldY = response.oldY;
		message.Response.playerAction.newX = response.newX;
		message.Response.playerAction.newY = response.newY;
		NetManager.Instance.SendMessageToAll(message, senderID);
		Log.ReducePerfixLength();
		//设置下一个玩家
		GameService.Instance.TurnToNextPlayer();
	}

	//客户端接收轮到哪个玩家出牌
	void OnTurnToPlayResponse(ulong senderID, TurnToPlayResponse response) {
		//设置当前出牌玩家
		GameService.Instance.TurnToPlayer = PlayerManager.Instance.FindPlayerBySeatID(response.seatID);
	}

	//客户端接收回合动作
	void OnPlayerActionResponse(ulong senderID, PlayerActionResponse response) {
		//使自己运行这个动作
		GameSceneBoardManager.Instance.HandleTurnAction(response.oldX, response.oldY, response.newX, response.newY);
	}
}
