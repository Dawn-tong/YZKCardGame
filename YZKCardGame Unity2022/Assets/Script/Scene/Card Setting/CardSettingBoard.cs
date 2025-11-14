using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CardSettingBoard : MonoBehaviour {
	CardManager cardManager;
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
	int clickPositionX;
	int clickPositionY;
	public delegate void TileClickDelegate(int positionX, int positionY);
	public TileClickDelegate OnTileClicked;
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			if (UIShield.IsClickBlockingUI()) {
				return;
			}
			clickPosition = Input.mousePosition;
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3Int cellPosition = boardTilemap.WorldToCell(worldPoint);
			clickPositionX = cellPosition.x;
			clickPositionY = cellPosition.y;
		}
		if (Input.GetMouseButtonUp(0)) {
			//如果松手时的位置与按下时的位置相近
			if(Vector2.Distance(clickPosition, Input.mousePosition) < 10f) {
				OnTileClicked?.Invoke(clickPositionX, clickPositionY);
			}	
		}
	}






	[SerializeField] GameObject chessPrefab;
	CardSettingChessInfo[,] chessBoard = new CardSettingChessInfo[10, 10];
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
		Card[] cardsList = cardManager.cardsList;
		Transform parent = boardTilemap.gameObject.transform;
		for (int i = 0; i < cardsList.Length; i++) {
			Card card = cardsList[i];
			if(card != null && card.positionX != -1) {
				GameObject chess = Instantiate(chessPrefab, parent);
				chess.transform.position = new Vector3(card.positionX * 1.1f, card.positionY * 1.1f, 0);
				CardSettingChessInfo cardSettingChessInfo = chess.GetComponent<CardSettingChessInfo>();
				cardSettingChessInfo.card = card;
				cardSettingChessInfo.index = i;
				cardSettingChessInfo.UpdateChessInfo();
				chessBoard[card.positionX, card.positionY] = cardSettingChessInfo;
			}
		}
	}
	//更新棋子
	public void UpdateBoardChess(int positionX, int positionY) {
		if (positionX == -1) {
			return;
		}
		chessBoard[positionX, positionY].UpdateChessInfo();
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