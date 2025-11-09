using UnityEngine;

/// <summary>
/// 摄像机拖拽控制器
/// 实现使用鼠标拖拽来移动摄像机的效果
/// </summary>
public class CameraDragController : MonoBehaviour
{
    [Header("边界设置")]
    [SerializeField] Vector2 leftDownEdge = new Vector2(-10f, -10f);
    [SerializeField] Vector2 rightUpEdge = new Vector2(10f, 10f);
	[Header("视野边缘百分比 (0-0.5)")]
	[SerializeField, Range(0f, 0.5f)] float leftViewportPercent = 0.2f;
	[SerializeField, Range(0f, 0.5f)] float rightViewportPercent = 0.2f;
	[SerializeField, Range(0f, 0.5f)] float topViewportPercent = 0.1f;
	[SerializeField, Range(0f, 0.5f)] float bottomViewportPercent = 0.1f;
	[Header("拖拽速度")]
	[SerializeField] float dragSpeed = 1.5f;
	[Header("缩放设置")]
	[SerializeField] float zoomSpeed = 0.6f;
	[SerializeField] float minZoom = 2f;
	[SerializeField] float maxZoom = 20f;
    





    Camera targetCamera;
    void Awake() {
        targetCamera = GetComponent<Camera>();
        if (targetCamera == null) {
            Debug.LogError("CameraDragController: 未找到Camera组件！");
        }
    }






	Vector3 lastMousePosition;
	bool isDragging = false;
	bool hasWarnedPerspective = false;
	void Update() {
        if (targetCamera == null)
            return;
        HandleDrag();
        HandleZoom();
    }
    private void HandleDrag() {
        // 鼠标按下开始拖拽
        if (Input.GetMouseButtonDown(0) && !UIManager.IsClickBlockingUI()) {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        // 鼠标抬起结束拖拽
        if (Input.GetMouseButtonUp(0)) {
            isDragging = false;
        }
        // 拖拽中
        if (isDragging && Input.GetMouseButton(0)) {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            // 将屏幕空间的移动转换为世界空间
			Vector3 move = new Vector3(-delta.x, -delta.y, 0) * dragSpeed * Time.deltaTime;
			move *= targetCamera.orthographicSize;
			Vector3 newPosition = transform.position + move;
			transform.position = ClampPositionToBounds(newPosition);
            lastMousePosition = Input.mousePosition;
        }
    }
    private void HandleZoom() {
        // 获取鼠标滚轮输入
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
		if (scrollDelta > 0.001f && !UIManager.IsClickBlockingUI()) {
            float newSize = targetCamera.orthographicSize / (1 + scrollDelta * zoomSpeed);
			targetCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
			transform.position = ClampPositionToBounds(transform.position);
		}
		if (scrollDelta < -0.001f && !UIManager.IsClickBlockingUI()) {
            float newSize = targetCamera.orthographicSize * (1 - scrollDelta * zoomSpeed);
			targetCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
			transform.position = ClampPositionToBounds(transform.position);
		}
    }
	Vector3 ClampPositionToBounds(Vector3 position) {
		Vector2 halfSize = GetCameraHalfSize();
		float leftRatio = ClampViewportPercent(leftViewportPercent);
		float rightRatio = ClampViewportPercent(rightViewportPercent);
		float bottomRatio = ClampViewportPercent(bottomViewportPercent);
		float topRatio = ClampViewportPercent(topViewportPercent);
		float minX = leftDownEdge.x + halfSize.x * (1f - 2f * leftRatio);
		float maxX = rightUpEdge.x - halfSize.x * (1f - 2f * rightRatio);
		float minY = leftDownEdge.y + halfSize.y * (1f - 2f * bottomRatio);
		float maxY = rightUpEdge.y - halfSize.y * (1f - 2f * topRatio);
		if (minX > maxX) {
			position.x = (leftDownEdge.x + rightUpEdge.x) * 0.5f;
		}
		else {
			position.x = Mathf.Clamp(position.x, minX, maxX);
		}
		if (minY > maxY) {
			position.y = (leftDownEdge.y + rightUpEdge.y) * 0.5f;
		}
		else {
			position.y = Mathf.Clamp(position.y, minY, maxY);
		}
		return position;
	}
	float ClampViewportPercent(float value) {
		return Mathf.Clamp(value, 0f, 0.5f);
	}
	Vector2 GetCameraHalfSize() {
		if (targetCamera == null)
			return Vector2.zero;
		if (targetCamera.orthographic) {
			float halfHeight = targetCamera.orthographicSize;
			float halfWidth = halfHeight * targetCamera.aspect;
			return new Vector2(halfWidth, halfHeight);
		}
		if (!hasWarnedPerspective) {
			hasWarnedPerspective = true;
			Debug.LogWarning("CameraDragController: 目前仅精准支持正交摄像机的边界约束。");
		}
		return Vector2.zero;
	}






    /// <summary>
    /// 设置边界
    /// </summary>
    public void SetBounds(Vector2 leftDown, Vector2 rightUp) {
        leftDownEdge = leftDown;
        rightUpEdge = rightUp;
		transform.position = ClampPositionToBounds(transform.position);
    }
    /// <summary>
    /// 设置缩放范围
    /// </summary>
    public void SetZoomRange(float min, float max) {
        minZoom = min;
        maxZoom = max;
        // 立即应用限制到当前摄像机
        if (targetCamera != null) {
            if (targetCamera.orthographic) {
                targetCamera.orthographicSize = Mathf.Clamp(targetCamera.orthographicSize, minZoom, maxZoom);
            }
            else {
                targetCamera.fieldOfView = Mathf.Clamp(targetCamera.fieldOfView, minZoom, maxZoom);
            }
			transform.position = ClampPositionToBounds(transform.position);
        }
    }
}

