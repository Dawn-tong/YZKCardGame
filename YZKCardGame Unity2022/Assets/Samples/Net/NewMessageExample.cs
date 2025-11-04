using Network;
using UnityEngine;
using ProtoMessage;

/// <summary>
/// 消息系统使用示例（ProtoBuf版本）
/// </summary>
public class MessageExample : MonoBehaviour
{
    private void Start()
    {
        // 订阅消息（根据当前proto文件定义）
        MessageDistributer<ulong>.Instance.Subscribe<PlayerJoinRoomRequest>(OnPlayerJoinRoomRequest);
        MessageDistributer<ulong>.Instance.Subscribe<PlayerJoinRoomResponse>(OnPlayerJoinRoomResponse);
    }
    
    private void OnDestroy()
    {
        // 取消订阅
        MessageDistributer<ulong>.Instance.Unsubscribe<PlayerJoinRoomRequest>(OnPlayerJoinRoomRequest);
        MessageDistributer<ulong>.Instance.Unsubscribe<PlayerJoinRoomResponse>(OnPlayerJoinRoomResponse);
    }
    
    // ==================== 消息处理器 ====================
    
    /// <summary>
    /// 处理玩家加入房间请求
    /// </summary>
    private void OnPlayerJoinRoomRequest(ulong senderId, PlayerJoinRoomRequest message)
    {
        Debug.Log($"[客户端] 收到玩家加入房间请求，发送者ID: {senderId}");
        // 处理玩家加入房间的逻辑...
        
        // 可以发送响应
        SendPlayerJoinRoomResponse();
    }
    
    /// <summary>
    /// 处理玩家加入房间响应
    /// </summary>
    private void OnPlayerJoinRoomResponse(ulong senderId, PlayerJoinRoomResponse message)
    {
        Debug.Log($"[客户端] 收到玩家加入房间响应，发送者ID: {senderId}");
        // 处理玩家加入房间响应的逻辑...
        // 例如：更新UI，显示房间内玩家列表等
    }
    
    // ==================== 发送消息示例 ====================
    
    /// <summary>
    /// 发送玩家加入房间请求
    /// </summary>
    public void SendPlayerJoinRoomRequest()
    {
        // 创建消息
        NetMessage message = new NetMessage
        {
            Request = new NetMessageRequest
            {
                playerJoinRoom = new PlayerJoinRoomRequest()
            }
        };
        
        // 序列化并发送
        byte[] data = PackageHandler.PackMessage(message);
        if (data != null)
        {
            Debug.Log("[客户端] 发送玩家加入房间请求");
            // 通过网络管理器发送
            // NetManager.Instance.SendMessage(data);
        }
    }
    
    /// <summary>
    /// 发送玩家加入房间响应
    /// </summary>
    public void SendPlayerJoinRoomResponse()
    {
        // 创建消息
        NetMessage message = new NetMessage
        {
            Response = new NetMessageResponse
            {
                PlayerJoinRoom = new PlayerJoinRoomResponse()
            }
        };
        
        // 序列化并发送
        byte[] data = PackageHandler.PackMessage(message);
        if (data != null)
        {
            Debug.Log("[服务器] 发送玩家加入房间响应");
            // 通过网络管理器发送
            // NetManager.Instance.SendMessageToAll(data);
        }
    }
    
    // ==================== 测试按钮 ====================
    
    // 可以从Unity Inspector或按钮调用
    [ContextMenu("测试发送加入房间请求")]
    public void TestSendJoinRequest()
    {
        SendPlayerJoinRoomRequest();
    }
}


