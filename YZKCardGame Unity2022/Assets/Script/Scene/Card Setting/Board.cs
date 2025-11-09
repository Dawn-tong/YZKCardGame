using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
	public int boardSize = 10;
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



	[SerializeField] Tilemap chessTilemap;
	public delegate void TileClickDelegate(int positionX, int positionY);
	public TileClickDelegate OnTileClicked;
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			if (UIManager.IsClickBlockingUI()) {
				return;
			}
			Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3Int cellPosition = boardTilemap.WorldToCell(worldPoint);
			// 检查该位置是否有棋子
			Debug.Log($"点击了{cellPosition}");
			if (chessTilemap.HasTile(cellPosition)) {
				OnTileClicked?.Invoke(cellPosition.x, cellPosition.y);
			}
		}
	}
	

}