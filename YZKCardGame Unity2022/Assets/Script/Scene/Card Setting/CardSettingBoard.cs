using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CardSettingBoard : MonoBehaviour {
	CardsListManager cardManager;
	[SerializeField] CardSettingUI_OneCardPanel oneCardPanel;
	public void OnSceneEnter() {
		//初始化cardManager
		cardManager = PlayerManager.Instance.currentPlayer.cardManager;
		//点击棋盘打开属性面板
		OnTileClicked += oneCardPanel.ClickBoardToOpenOneCardPanel;
		//刷新棋盘
		UpdateBoard();
	}
	public void OnSceneLeave() {
		OnTileClicked -= oneCardPanel.ClickBoardToOpenOneCardPanel;
	}






	//			public int boardSize = 10;
	[SerializeField] Tilemap boardTilemap;
	//			[SerializeField] TileBase boardTile;
	//			void Start() {
	//				boardTilemap.ClearAllTiles(); // 清空现有棋盘
	//				for (int x = 0; x < boardSize; x++) {
	//					for (int y = 0; y < boardSize; y++) {
	//						Vector3Int cellPosition = new Vector3Int(x, y, 0);
	//						boardTilemap.SetTile(cellPosition, boardTile);
	//					}
	//				}
	//				Debug.Log($"{boardSize}*{boardSize}棋盘生成完毕");
	//			}



	Vector3 clickPosition;
	float clickTime = 0;
	Timer holdTimer;
	bool existCard = false;
	Card clickCard;
	GameObject clickCardObject;
	int clickPositionX;
	int clickPositionY;
	public delegate void TileClickDelegate(int positionX, int positionY);
	public TileClickDelegate OnTileClicked;
	void Update() {
		if (Input.GetMouseButtonDown(0) && !UIShield.IsClickBlockingUI()) {
			MouseDown();
		}
		//长按拿起棋子
		if(existCard) {
			MouseHolding();
		}
		if (Input.GetMouseButtonUp(0) && !UIShield.IsClickBlockingUI()) {
			MouseUp();
		}
	}
	//鼠标按下
	void MouseDown() {
		clickPosition = Input.mousePosition;
		clickTime = Time.time;
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3Int cellPosition = boardTilemap.WorldToCell(worldPoint);
		clickPositionX = cellPosition.x;
		clickPositionY = cellPosition.y;
		//如果格子内有棋子
		if (clickPositionX < 0 || clickPositionX > 3 || clickPositionY < 0 || clickPositionY > 3) {
			return;
		}
		if(chessBoard[clickPositionX, clickPositionY] == null) {
			return;
		}
		//则创建长按计时器
		holdTimer = TimerManager.CreateTimer().SetName("HoldTimer").SetTime(0.6f).SetCount(1).SetAction(
			() => {
				//判断鼠标是否还在原来的格子
				Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector3Int cellPosition = boardTilemap.WorldToCell(worldPoint);
				if(cellPosition.x != clickPositionX || cellPosition.y != clickPositionY) {
					return;
				}
				//拿起棋子
				existCard = true;
				clickCard = cardManager.FindCardByPosition(clickPositionX, clickPositionY);
				clickCardObject = chessBoard[clickPositionX, clickPositionY].gameObject;
				clickCardObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
			}
		);
	}
	//鼠标持续
	void MouseHolding() {
		//设置棋子位置为鼠标位置
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		clickCardObject.transform.position = new Vector3(worldPoint.x, worldPoint.y, -10f);
	}
	//鼠标抬起
	void MouseUp() {
		Timer.SafeDelete(holdTimer);
		//拖拽
		if(existCard && Time.time - clickTime > 0.6f) {
			clickCardObject.transform.position = new Vector3(clickCardObject.transform.position.x, clickCardObject.transform.position.y, 0);
			clickCardObject.transform.localScale = new Vector3(1f, 1f, 1f);
			//放下棋子
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3Int cellPosition = boardTilemap.WorldToCell(worldPoint);
			if(cellPosition.x >= 0 && cellPosition.x < 4 && cellPosition.y >= 0 && cellPosition.y < 4) {
				PutCardOnBoard(clickCard, cellPosition.x, cellPosition.y);
			}
			else{
				PutCardOnBoard(clickCard, clickPositionX, clickPositionY);
			}
		}
		//点击
		else{
			if(Vector2.Distance(clickPosition, Input.mousePosition) < 10f) {
				OnTileClicked?.Invoke(clickPositionX, clickPositionY);
			}
		}
		clickTime = 0;
		existCard = false;
		clickCard = null;
		clickCardObject = null;
	}






	[SerializeField] GameObject chessPrefab;
	Chess[,] chessBoard = new Chess[10, 10];
	//放置棋子
	public void PutCardOnBoard(Card card, int positionX, int positionY) {
		cardManager.PutCardToPosition(card, positionX, positionY);
		UpdateBoard();
	}
	//更新棋盘
	public void UpdateBoard() {
		Debug.Log("更新所有棋盘事件发生");
		for (int x = 0; x < 10; x++) {
			for (int y = 0; y < 10; y++) {
				if(chessBoard[x, y] != null) {
					Destroy(chessBoard[x, y].gameObject);
					chessBoard[x, y] = null;
				}
			}
		}
		Card[] cardsList = cardManager.cardsArray;
		Transform parent = boardTilemap.gameObject.transform;
		for (int i = 0; i < cardsList.Length; i++) {
			Card card = cardsList[i];
			if(card != null && card.positionX != -1) {
				GameObject chess = Instantiate(chessPrefab, parent);
				chess.transform.position = new Vector3(card.positionX * 1.1f + 0.5f, card.positionY * 1.1f + 0.5f, 0);
				Chess cardSettingChessInfo = chess.GetComponent<Chess>();
				cardSettingChessInfo.card = card;
				cardSettingChessInfo.ShowIndex();
				cardSettingChessInfo.UpdateChess();
				chessBoard[card.positionX, card.positionY] = cardSettingChessInfo;
			}
		}
	}
	//更新棋子
	public void UpdateBoardChess(int positionX, int positionY) {
		if (positionX == -1) {
			return;
		}
		chessBoard[positionX, positionY].UpdateChess();
	}
	//删除棋子
	public void DeleteBoardChess(int positionX, int positionY) {
		if (positionX == -1) {
			return;
		}
		Destroy(chessBoard[positionX, positionY].gameObject);
		chessBoard[positionX, positionY] = null;
	}
}