using UnityEngine;

public class ChessPiece : MonoBehaviour
{
	[Header("棋子属性")]
	public PieceType pieceType;
	public PieceColor pieceColor;
	public int gridX;
	public int gridY;

	private SpriteRenderer spriteRenderer;
	private Vector3 originalScale;

	public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
	public enum PieceColor { White, Black }

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		originalScale = transform.localScale;

		// 根据棋子颜色设置视觉
		UpdateVisual();
	}

	void UpdateVisual()
	{
		if (spriteRenderer != null)
		{
			spriteRenderer.color = pieceColor == PieceColor.White ? Color.white : Color.black;
		}
	}

	public void OnSelected()
	{
		Debug.Log($"选中棋子: {pieceColor} {pieceType} 在位置 ({gridX}, {gridY})");

		// 选中效果
		StartCoroutine(SelectionAnimation());

		// 显示可移动位置等逻辑
		ShowAvailableMoves();
	}

	private System.Collections.IEnumerator SelectionAnimation()
	{
		// 缩放动画
		transform.localScale = originalScale * 1.2f;
		yield return new WaitForSeconds(0.1f);
		transform.localScale = originalScale;
	}

	void ShowAvailableMoves()
	{
		// 这里实现显示棋子可移动位置的逻辑
		// 可以通过高亮格子等方式显示
	}

	public void MoveTo(int targetX, int targetY)
	{
		gridX = targetX;
		gridY = targetY;

		// 移动到新位置
		ChessBoard board = FindObjectOfType<ChessBoard>();
		if (board != null)
		{
			transform.position = board.GetWorldPosition(targetX, targetY);
		}
	}
}
