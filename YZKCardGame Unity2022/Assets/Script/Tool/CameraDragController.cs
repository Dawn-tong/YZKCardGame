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
    [Header("拖拽速度")]
    [SerializeField] float dragSpeed = 1.5f;
    [Header("缩放设置")]
    [SerializeField] float zoomSpeed = 0.8f;
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
	void Update() {
        if (targetCamera == null)
            return;
        HandleDrag();
        HandleZoom();
    }
    private void HandleDrag() {
        // 鼠标按下开始拖拽
        if (Input.GetMouseButtonDown(0)) {
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
            // 限制在边界内
            newPosition.x = Mathf.Clamp(newPosition.x, leftDownEdge.x, rightUpEdge.x);
            newPosition.y = Mathf.Clamp(newPosition.y, leftDownEdge.y, rightUpEdge.y);
            transform.position = newPosition;
            lastMousePosition = Input.mousePosition;
        }
    }
    private void HandleZoom() {
        // 获取鼠标滚轮输入
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta > 0.001f) {
            float newSize = targetCamera.orthographicSize / (1 + scrollDelta * zoomSpeed);
			targetCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
		}
        if (scrollDelta < -0.001f) {
            float newSize = targetCamera.orthographicSize * (1 - scrollDelta * zoomSpeed);
			targetCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
		}
    }






    /// <summary>
    /// 设置边界
    /// </summary>
    public void SetBounds(Vector2 leftDown, Vector2 rightUp) {
        leftDownEdge = leftDown;
        rightUpEdge = rightUp;
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
        }
    }
}

