using UnityEngine;
using System.Collections.Generic;

public class ChessBoard : MonoBehaviour
{
	[Header("棋盘设置")]
	public int boardSize = 10; // 棋盘大小
	public float cellSize = 1f; // 格子大小
	public GameObject cellPrefab; // 格子预制体
	public LayerMask boardLayer; // 棋盘层级

	private ChessCell[,] cells;
	private Camera mainCamera;

	void Start()
	{
		mainCamera = Camera.main;
		CreateBoard();
	}

	void CreateBoard()
	{
		cells = new ChessCell[boardSize, boardSize];

		for (int x = 0; x < boardSize; x++)
		{
			for (int y = 0; y < boardSize; y++)
			{
				// 创建格子
				Vector3 position = new Vector3(
					x * cellSize - (boardSize * cellSize) / 2f + cellSize / 2f,
					y * cellSize - (boardSize * cellSize) / 2f + cellSize / 2f,
					0f
				);

				GameObject cellObj = Instantiate(cellPrefab, position, Quaternion.identity, transform);
				cellObj.name = $"Cell_{x}_{y}";

				// 获取或添加ChessCell组件
				ChessCell cell = cellObj.GetComponent<ChessCell>();
				if (cell == null)
					cell = cellObj.AddComponent<ChessCell>();

				cell.Initialize(x, y, this);
				cells[x, y] = cell;

				// 设置格子颜色（棋盘格样式）
				SetCellColor(cellObj, x, y);
			}
		}
	}

	void SetCellColor(GameObject cellObj, int x, int y)
	{
		SpriteRenderer renderer = cellObj.GetComponent<SpriteRenderer>();
		if (renderer != null)
		{
			bool isLight = (x + y) % 2 == 0;
			renderer.color = isLight ? new Color(1f, 0.9f, 0.8f) : new Color(0.6f, 0.4f, 0.2f);
		}
	}

	void Update()
	{
		HandleInput();
		HandleBoardMovement();
	}

	void HandleInput()
	{
		if (Input.GetMouseButtonDown(0)) // 左键点击
		{
			Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, boardLayer);

			if (hit.collider != null)
			{
				ChessCell clickedCell = hit.collider.GetComponent<ChessCell>();
				if (clickedCell != null)
				{
					OnCellClicked(clickedCell);
				}
			}
		}

	}

	public void OnCellClicked(ChessCell cell)
	{
		Debug.Log($"棋盘格子被点击: ({cell.gridX}, {cell.gridY})");

		// 在这里处理棋子选择逻辑
		SelectPieceAt(cell.gridX, cell.gridY);
	}

	void SelectPieceAt(int x, int y)
	{
		// 查找该位置上的棋子
		ChessPiece piece = FindPieceAt(x, y);
		if (piece != null)
		{
			// 选中棋子逻辑
			piece.OnSelected();
		}
		else
		{
			// 空位置点击逻辑
			Debug.Log($"位置 ({x}, {y}) 没有棋子");
		}
	}

	ChessPiece FindPieceAt(int x, int y)
	{
		// 这里实现查找棋子的逻辑
		// 可以通过物理检测或者维护一个棋子位置字典来实现
		Collider2D[] colliders = Physics2D.OverlapPointAll(GetWorldPosition(x, y));
		foreach (Collider2D collider in colliders)
		{
			ChessPiece piece = collider.GetComponent<ChessPiece>();
			if (piece != null)
				return piece;
		}
		return null;
	}

	public Vector3 GetWorldPosition(int x, int y)
	{
		return new Vector3(
			x * cellSize - (boardSize * cellSize) / 2f + cellSize / 2f,
			y * cellSize - (boardSize * cellSize) / 2f + cellSize / 2f,
			0f
		);
	}

	// 处理棋盘移动和缩放
	void HandleBoardMovement()
	{
		// 鼠标滚轮缩放
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll != 0)
		{
			mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - scroll * 2f, 2f, 10f);
		}

		// 鼠标中键拖拽移动
		if (Input.GetMouseButton(2))
		{
			float speed = 0.01f * mainCamera.orthographicSize;
			Vector3 delta = new Vector3(-Input.GetAxis("Mouse X") * speed, -Input.GetAxis("Mouse Y") * speed, 0);
			mainCamera.transform.Translate(delta, Space.World);
		}
	}
}