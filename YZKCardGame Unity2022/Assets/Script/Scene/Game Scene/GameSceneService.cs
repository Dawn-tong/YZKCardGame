using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameSceneService {
	static GameSceneService instance;
	public static GameSceneService Instance {
		get {
			if (instance == null)
				instance = new GameSceneService();
			return instance;
		}
	}






	//选择棋盘位置
	public void ChooseBoardPosition() {
		Debug.Log("选择棋盘位置");
		//玩家选择棋盘位置(此处先写两个玩家的情况)
		if(NetManager.Instance.isHostPlayer) {
			//设置非主机玩家为右上角
			for (int i = 0; i < PlayerManager.Instance.allPlayers.Length; i++) {
				if(PlayerManager.Instance.allPlayers[i] == null) {
					continue;
				}
				PlayerManager.Instance.allPlayers[i].SetCornerID(CornerID.TopRight);
			}
			//设置主机玩家为左下角
			PlayerManager.Instance.currentPlayer.SetCornerID(CornerID.BottomLeft);
		}
		else{	
			//玩家默认为左下角,所以不需要设置主机玩家
			//设置自己为右上角
			PlayerManager.Instance.currentPlayer.SetCornerID(CornerID.TopRight);
		}
		//TODO:多人情况
		//选择结束后开始游戏
		StartGame();
	}
	
	public void StartGame() {
		GameCommunicateService.Instance.BeforeStartGame();
		Debug.Log("创建棋盘");
		if (PlayerManager.Instance.GetPlayerCount() <= 2) {
			GameSceneBoardManager.Instance.InitBoard(10);
			CameraDragManager.Instance.SetCurrentController(new Vector2(0, 0), new Vector2(10.9f, 10.9f), 2f, 7f);
		}
		else{
			GameSceneBoardManager.Instance.InitBoard(12);
			CameraDragManager.Instance.SetCurrentController(new Vector2(0, 0), new Vector2(13.1f, 13.1f), 2f, 8f);
		}
		Debug.Log("刷新玩家卡片位置信息");
		RefreshCardsPosition();
		Debug.Log("将玩家卡牌放置在棋盘上");
		//创建自己棋子
		foreach(Card card in PlayerManager.Instance.currentPlayer.gameCardManager.cardsArray) {
			if(card == null || !card.exists) {
				continue;
			}
			GameSceneBoardManager.Instance.CreateSelfChess(card);
		}
		//创建其他玩家棋子
		foreach (Player player in PlayerManager.Instance.allPlayers) {
			if(player == null || player == PlayerManager.Instance.currentPlayer) {
				continue;
			}
			foreach (Card card in player.gameCardManager.cardsArray) {
				if(card == null || !card.exists) {
					continue;
				}
				GameSceneBoardManager.Instance.CreateOtherChess(card);
			}
		}
		//更新UI
		Debug.Log("更新UI");
		GameSceneUI.Instance.UpdateUI();
		//服务器随机选择一个玩家作为第一个出牌玩家
		if(NetManager.Instance.isHostPlayer) {
			GameService.Instance.RandomChooseFirstPlayer();
			GameSceneUI.Instance.UpdateCurrentTurnText(GameService.Instance.TurnToPlayer.cornerID);
		}
		Debug.Log("StartGame结束");
	}
	

	
	//根据自己所在的角落刷新卡组位置信息
	public void RefreshCardsPosition() {
		// 中心点坐标
		float halfSideLength = GameSceneBoardManager.Instance.currentBoardSideLength / 2f - 0.5f;
		Vector2 center = new Vector2(halfSideLength, halfSideLength);
		foreach (Player player in PlayerManager.Instance.allPlayers) {
			if(player == null) {
				continue;
			}
			float rotate = ((int)player.cornerID - 1) * 90f;
			foreach (Card card in player.cardManager.cardsArray) {
				if(card == null || !card.exists) {
					continue;
				}
				//创建卡片副本
				Card gameCard = new Card(card);
				player.gameCardManager.cardsArray[gameCard.index] = gameCard;
				// 将卡牌坐标转换为以中心点为原点的相对坐标
				Vector2 relativePos = new Vector2(gameCard.positionX - center.x, gameCard.positionY - center.y);
				// 根据旋转角度进行坐标变换
				Vector2 transformedPos = RotateVector(relativePos, -rotate); // 负号表示反向旋转
				// 转换回绝对坐标
				gameCard.positionX = Mathf.RoundToInt(transformedPos.x + center.x);
				gameCard.positionY = Mathf.RoundToInt(transformedPos.y + center.y);
			}
		}

	}
	// 旋转向量方法
	Vector2 RotateVector(Vector2 vector, float degrees) {
		float radians = degrees * Mathf.Deg2Rad;
		float cos = Mathf.Cos(radians);
		float sin = Mathf.Sin(radians);
		return new Vector2(
			vector.x * cos - vector.y * sin,
			vector.x * sin + vector.y * cos
		);
	}
}
