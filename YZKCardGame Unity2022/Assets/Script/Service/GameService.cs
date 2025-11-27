using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameService {
	static GameService instance;
	public static GameService Instance {
		get {
			if (instance == null)
				instance = new GameService();
			return instance;
		}
	}






	Player turnToPlayer;	//当前出牌玩家
	public Player TurnToPlayer {
		get {
			return turnToPlayer;
		}
		set {
			turnToPlayer = value;
			Debug.Log($"Set TurnToPlayer: seatID={turnToPlayer.seatID}");
			UIMessagePanel.Instance.AddMessage($"Set TurnToPlayer: seatID={turnToPlayer.seatID}");
			//更新当前回合玩家
			GameSceneUI.Instance.UpdateCurrentTurnText(turnToPlayer.cornerID);
			//如果是自己那么就回合开始
			if(turnToPlayer == PlayerManager.Instance.currentPlayer) {
				OnTurnStart();
			}
			//如果是服务器则通知所有玩家
			if (NetManager.Instance.isHostPlayer) {
				GameCommunicateService.Instance.SendTurnToPlayResponse(turnToPlayer);
			}
		}
	}
	//随机选择一个玩家作为起点
	public void RandomChooseFirstPlayer() {
		TurnToPlayer = PlayerManager.Instance.GetRandomPlayer();
	}
	//轮到下一个玩家
	public void TurnToNextPlayer() {
		TurnToPlayer = PlayerManager.Instance.GetNextPlayer(TurnToPlayer);
	}







	//回合开始
	public void OnTurnStart() {
		//UIManager.Instance.CreateUI<UIMessage>().InitUIMessage("提示","回合开始");
		GameSceneBoardManager.Instance.enabled = true;
	}
	//回合结束
	public void OnTurnEnd() {
		GameSceneBoardManager.Instance.enabled = false;
	}






	//卡片攻击
	public void CardAttack(Card attacker, Card defender) {
		if (attacker.cardType == CardType.Bomb) {
			CardLoseHp(defender, 20);
		}
		else {
			CardLoseHp(defender, attacker.atk);
		}
		if (defender.cardType == CardType.Bomb) {
			CardLoseHp(attacker, 20);
		}
		else {
			CardLoseHp(attacker, defender.atk);
		}
	}
	//卡片损失生命
	public void CardLoseHp(Card card, int atk) {
		card.hp -= atk;
		card.chessComponent.UpdateChess();
		if(card.hp <= 0) {
			CardDie(card);
		}
	}
	//卡片死亡
	public void CardDie(Card card) {
		Chess chess = card.chessComponent;
		if(chess != null) {
			chess.DestroyChess();
		}
	}
}
