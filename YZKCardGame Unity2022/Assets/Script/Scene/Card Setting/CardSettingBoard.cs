using UnityEngine;
using UnityEngine.Tilemaps;

public class CardSettingBoard : MonoBehaviour {
	void Awake() {
		SceneLoaderManager.Instance.RegisterSceneEnterCallback(Scene.CardSetting, OnSceneEnter);
		SceneLoaderManager.Instance.RegisterSceneLeaveCallback(Scene.CardSetting, OnSceneLeave);
	}

	CardManager cardManager;
	//注册交换事件
	public void OnSceneEnter() {
		cardManager = PlayerManager.Instance.currentPlayer.cardManager;
		cardManager.OnCardSwap += UpdateBoard;
		UpdateBoard();
	}
	//取消注册交换事件
	public void OnSceneLeave() {
		SceneLoaderManager.Instance.UnregisterSceneEnterCallback(Scene.CardSetting, OnSceneEnter);
		SceneLoaderManager.Instance.UnregisterSceneLeaveCallback(Scene.CardSetting, OnSceneLeave);
		cardManager.OnCardSwap -= UpdateBoard;
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
			if (UIManager.IsClickBlockingUI()) {
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






	//更新棋盘
	[SerializeField] GameObject chessPrefab;
	CardSettingChessInfo[,] chessBoard = new CardSettingChessInfo[10, 10];
	void UpdateBoard() {
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
}