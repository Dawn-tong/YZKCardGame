using UnityEngine;

public class ChessCell : MonoBehaviour
{
	[HideInInspector] public int gridX;
	[HideInInspector] public int gridY;

	private ChessBoard board;
	private SpriteRenderer spriteRenderer;
	private Color originalColor;

	public void Initialize(int x, int y, ChessBoard chessBoard)
	{
		gridX = x;
		gridY = y;
		board = chessBoard;
		spriteRenderer = GetComponent<SpriteRenderer>();
		originalColor = spriteRenderer.color;

		// 设置碰撞器大小
		BoxCollider2D collider = GetComponent<BoxCollider2D>();
		if (collider != null)
		{
			collider.size = Vector2.one * board.cellSize * 0.9f;
		}
	}

	void OnMouseEnter()
	{
		// 鼠标悬停效果
		spriteRenderer.color = new Color(0.8f, 1f, 0.8f, 0.8f);
	}

	void OnMouseExit()
	{
		// 恢复原色
		spriteRenderer.color = originalColor;
	}

	void OnMouseDown()
	{
		// 确保点击在格子内部
		board.OnCellClicked(this);
	}
}
