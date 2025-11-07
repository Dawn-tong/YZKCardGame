using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
	[SerializeField] private Tilemap boardTilemap;
	[SerializeField] private Tilemap chessTilemap;

	public delegate void TileClickDelegate(int positionX, int positionY);
	public TileClickDelegate OnTileClicked;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			HandleTileClick();
		}
	}

	private void HandleTileClick()
	{
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3Int cellPosition = boardTilemap.WorldToCell(worldPoint);
		// 检查该位置是否有棋子
		if (chessTilemap.HasTile(cellPosition))
		{
			OnTileClicked?.Invoke(cellPosition.x, cellPosition.y);
		}
	}
}