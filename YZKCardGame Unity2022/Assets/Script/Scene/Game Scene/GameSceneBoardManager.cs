using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameSceneBoardManager : MonoBehaviour {
	static GameSceneBoardManager instance;
	public static GameSceneBoardManager Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<GameSceneBoardManager>();
			}
			return instance; 
		}
	}






	//棋盘
	[SerializeField] Tilemap currentBoard;
	public int currentBoardSideLength;
	public int upEdge;
	public int downEdge;
	public int leftEdge;
	public int rightEdge;
	//棋子
	[SerializeField] GameObject chessPrefab;
	public GameObject[,] chessObjects = new GameObject[12, 12];
	//棋子对应的卡牌
	Dictionary<GameObject, Card> chessToCard = new Dictionary<GameObject, Card>();







	//初始化棋盘
	public void InitBoard(int sideLength) {
		currentBoardSideLength = sideLength;
		upEdge = sideLength - 1;
		downEdge = 0;
		leftEdge = 0;
		rightEdge = sideLength - 1;
		RefreshBoardSprite();
	}

	//创建棋子
	public void CreateSelfChess(Card card) {
		CreateChess(card);
	}
	public void CreateOtherChess(Card card) {
		GameObject chessObject = CreateChess(card);
		chessObject.GetComponent<Chess>().SetColor(new Color(0.57f, 0.42f, 0.41f));
	}
	GameObject CreateChess(Card card) {
		//创建
		GameObject chessObject = Instantiate(chessPrefab, currentBoard.transform);
		//赋值
		chessToCard[chessObject] = card;
		chessObject.GetComponent<Chess>().card = card;
		card.chessComponent = chessObject.GetComponent<Chess>();
		//刷新
		chessObjects[card.positionX, card.positionY] = chessObject;
		chessObject.transform.position = new Vector3(card.positionX * 1.1f + 0.5f, card.positionY * 1.1f + 0.5f, 0);
		chessObject.GetComponent<Chess>().UpdateChess();
		return chessObject;
	}

	//设置棋子位置
	public void MoveChess(int oldX, int oldY, int newX, int newY) {
		GameObject chess = chessObjects[oldX, oldY];
		Card card = chessToCard[chess];
		chessObjects[oldX, oldY] = null;
		chessObjects[newX, newY] = chess;
		CheckEdgeX(oldX);
		CheckEdgeY(oldY);
		RefreshCameraDrag();
		chess.transform.position = new Vector3(newX * 1.1f + 0.5f, newY * 1.1f + 0.5f, 0);
		card.positionX = newX;
		card.positionY = newY;
	}
	//判定原有位置的行列是否是最外侧,若为最外侧且这一行/列没有棋子,则重新设置行列的最外侧
	public void CheckEdgeX(int x) {
		if(upEdge <= downEdge) {
			return;
		}
		if (x == rightEdge) {
			for (int i = 0; i < 12; i++) {
				if (chessObjects[x, i] != null) {
					return;
				}
			}
			rightEdge--;
			RefreshBoardSprite();
			CheckEdgeX(rightEdge);
		}
		if (x == leftEdge) {
			for (int i = 0; i < 12; i++) {
				if (chessObjects[x, i] != null) {
					return;
				}
			}
			leftEdge++;
			RefreshBoardSprite();
			CheckEdgeX(leftEdge);
		}
	}
	public void CheckEdgeY(int y) {
		if(rightEdge <= leftEdge) {
			return;
		}
		if (y == upEdge) {
			for (int i = 0; i < 12; i++) {
				if (chessObjects[i, y] != null) {
					return;
				}
			}
			upEdge--;
			RefreshBoardSprite();
			CheckEdgeY(upEdge);
		}
		if (y == downEdge) {
			for (int i = 0; i < 12; i++) {
				if (chessObjects[i, y] != null) {
					return;
				}
			}
			downEdge++;
			RefreshBoardSprite();
			CheckEdgeY(downEdge);
		}
	}
	public void RefreshCameraDrag() {
		Vector2 leftDownEdge = new Vector2(leftEdge * 1.1f, downEdge * 1.1f);
		Vector2 rightUpEdge = new Vector2(rightEdge * 1.3f-0.1f, upEdge * 1.3f-0.1f);
		CameraDragManager.Instance.SetCurrentController(leftDownEdge, rightUpEdge);
	}
	//超出边界的部分将删除图片
	void RefreshBoardSprite() {
		for (int x = 0; x < 12; x++) {
			for (int y = 0; y < 12; y++) {
				if (x < leftEdge || x > rightEdge || y < downEdge || y > upEdge) {
					currentBoard.SetTile(new Vector3Int(x, y, 0), null);
				}
			}
		}
	}



	//攻击
	public void AttackChess(int oldX, int oldY, int newX, int newY) {
		//显示双方卡牌
		Chess oldChess = chessObjects[oldX, oldY].GetComponent<Chess>();
		Chess newChess = chessObjects[newX, newY].GetComponent<Chess>();
		oldChess.ShowInfo();
		newChess.ShowInfo();
		string oldCardInfo = oldChess.card.CardInfoToString();
		string newCardInfo = newChess.card.CardInfoToString();
		UIManager.Instance.CreateUI<UIMessage>().InitUIMessage("战斗信息", oldCardInfo + "\n攻击了\n" + newCardInfo);
		GameService.Instance.CardAttack(oldChess.card, newChess.card);
	}

	//处理回合动作
	public void HandleTurnAction(int oldX, int oldY, int newX, int newY) {
		//如果新位置没有棋子那么就移动
		if(chessObjects[newX, newY] == null) {
			MoveChess(oldX, oldY, newX, newY);
		}
		//如果新位置有棋子那么就攻击
		else {
			AttackChess(oldX, oldY, newX, newY);
		}
	}






	Vector3 clickPosition;
	int oldX;
	int oldY;
	List<Vector2> moveablePositions = new List<Vector2>();  //可移动位置
	[SerializeField] GameObject flickerPrefab;     //可移动位置UI
	List<GameObject> allFlickerObject = new List<GameObject>();   //可移动位置所有UI

	void Update() {
		if (Input.GetMouseButtonDown(0) && !UIShield.IsClickBlockingUI()) {
			MouseDown();
		}
		if (Input.GetMouseButtonUp(0) && !UIShield.IsClickBlockingUI()) {
			MouseUp();
		}
	}
	void MouseDown() {
		clickPosition = Input.mousePosition;
	}
	void MouseUp() {
		if(Vector2.Distance(clickPosition, Input.mousePosition) < 10f) {
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3Int cellPosition = currentBoard.WorldToCell(worldPoint);
			int clickPositionX = cellPosition.x;
			int clickPositionY = cellPosition.y;
			//如果越界则取消
			if(clickPositionX < leftEdge || clickPositionX > rightEdge || clickPositionY < downEdge || clickPositionY > upEdge) {
				CloseMoveChessPanel();
				return;
			}
			//如果点击到可以移动到的位置,则移动棋子
			bool isMoveablePosition = false;
			foreach(Vector2 moveablePosition in moveablePositions) {
				if(moveablePosition.x == clickPositionX && moveablePosition.y == clickPositionY) {
					//移动棋子
					HandleTurnAction(oldX, oldY, clickPositionX, clickPositionY);
					GameCommunicateService.Instance.SendTurnAction(oldX, oldY, clickPositionX, clickPositionY);
					GameService.Instance.OnTurnEnd();
					isMoveablePosition = true;
					break;
				}
			}
			CloseMoveChessPanel();
			if(isMoveablePosition) {
				return;
			}
			//如果点击位置存在棋子
			if(chessObjects[clickPositionX, clickPositionY] == null) {
				return;
			}
			//且是自己的棋子
			Card card = chessToCard[chessObjects[clickPositionX, clickPositionY]];
			if(!card.isSelfCard) {
				return;
			}
			//那么就打开移动棋子面板并记录位置
			oldX = clickPositionX;
			oldY = clickPositionY;
			OpenMoveChessPanel(clickPositionX, clickPositionY);
		}
	}
	void OpenMoveChessPanel(int positionX, int positionY) {
		if(upEdge >= positionY + 1) {
			IsMoveablePosition(positionX, positionY + 1);
		}
		if(downEdge <= positionY - 1) {
			IsMoveablePosition(positionX, positionY - 1);
		}
		if(leftEdge <= positionX - 1) {
			IsMoveablePosition(positionX - 1, positionY);
		}
		if(rightEdge >= positionX + 1) {
			IsMoveablePosition(positionX + 1, positionY);
		}
		if(moveablePositions.Count == 0) {
			CloseMoveChessPanel();
		}
	}
	void IsMoveablePosition(int positionX, int positionY) {
		if(chessObjects[positionX, positionY] == null || !chessToCard[chessObjects[positionX, positionY]].isSelfCard) {
			moveablePositions.Add(new Vector2(positionX, positionY));
			//创建UI对象
			GameObject obj = Instantiate(flickerPrefab);
			obj.transform.position = new Vector3(positionX * 1.1f + 0.5f, positionY * 1.1f + 0.5f, -1);
			allFlickerObject.Add(obj);
		}
	}
	//关闭移动棋子面板
	void CloseMoveChessPanel() {
		moveablePositions.Clear();
		foreach(GameObject obj in allFlickerObject) {
			Destroy(obj);
		}
		allFlickerObject.Clear();
	}
}
