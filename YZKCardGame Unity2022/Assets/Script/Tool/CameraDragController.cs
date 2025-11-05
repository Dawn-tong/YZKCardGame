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
    [SerializeField] float dragSpeed = 150f;
    





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
            // 根据摄像机类型调整移动
            if (targetCamera.orthographic) {
                // 正交摄像机
                move *= targetCamera.orthographicSize * 0.01f;
            }
            else {
                // 透视摄像机
                move *= 0.01f;
            }
            // 应用移动
            Vector3 newPosition = transform.position + move;
            // 限制在边界内
            newPosition.x = Mathf.Clamp(newPosition.x, leftDownEdge.x, rightUpEdge.x);
            newPosition.y = Mathf.Clamp(newPosition.y, leftDownEdge.y, rightUpEdge.y);
            transform.position = newPosition;
            lastMousePosition = Input.mousePosition;
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
    /// 设置拖拽速度
    /// </summary>
    public void SetDragSpeed(float speed) {
        dragSpeed = speed;
    }
}

