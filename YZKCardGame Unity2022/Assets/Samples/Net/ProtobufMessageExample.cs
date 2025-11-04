using System;
using System.IO;
using UnityEngine;
using ProtoMessage;
using ProtoBuf;

/// <summary>
/// ProtoBuf 消息使用示例
/// </summary>
public class ProtobufMessageExample : MonoBehaviour
{
    /// <summary>
    /// 示例：创建并序列化一个PlayerJoinRoom请求消息
    /// </summary>
    public byte[] CreatePlayerJoinRoomRequest()
    {
        // 1. 创建请求对象
        var joinRoomRequest = new PlayerJoinRoomRequest();
        
        // 2. 创建NetMessageRequest并设置请求
        var messageRequest = new NetMessageRequest
        {
            playerJoinRoom = joinRoomRequest
        };
        
        // 3. 创建NetMessage并设置Request
        var netMessage = new NetMessage
        {
            Request = messageRequest
        };
        
        // 4. 序列化为字节数组
        byte[] messageBytes = SerializeMessage(netMessage);
        
        Debug.Log($"消息序列化成功，大小: {messageBytes.Length} 字节");
        
        return messageBytes;
    }
    
    /// <summary>
    /// 示例：创建并序列化一个PlayerJoinRoom响应消息
    /// </summary>
    public byte[] CreatePlayerJoinRoomResponse()
    {
        // 1. 创建响应对象
        var joinRoomResponse = new PlayerJoinRoomResponse();
        
        // 2. 创建NetMessageResponse并设置响应
        var messageResponse = new NetMessageResponse
        {
            PlayerJoinRoom = joinRoomResponse
        };
        
        // 3. 创建NetMessage并设置Response
        var netMessage = new NetMessage
        {
            Response = messageResponse
        };
        
        // 4. 序列化为字节数组
        byte[] messageBytes = SerializeMessage(netMessage);
        
        Debug.Log($"响应序列化成功，大小: {messageBytes.Length} 字节");
        
        return messageBytes;
    }
    
    /// <summary>
    /// 将NetMessage序列化为字节数组
    /// </summary>
    public byte[] SerializeMessage(NetMessage message)
    {
        try
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, message);
                return stream.ToArray();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"消息序列化失败: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 将字节数组反序列化为NetMessage
    /// </summary>
    public NetMessage DeserializeMessage(byte[] data)
    {
        try
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<NetMessage>(stream);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"消息反序列化失败: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 完整示例：发送和接收消息
    /// </summary>
    public void CompleteExample()
    {
        Debug.Log("=== ProtoBuf 消息示例 ===");
        
        // 发送方：创建并序列化请求
        Debug.Log("1. 创建请求消息...");
        byte[] requestBytes = CreatePlayerJoinRoomRequest();
        
        // 模拟网络传输...
        Debug.Log("2. 通过网络发送消息...");
        
        // 接收方：反序列化请求
        Debug.Log("3. 接收并解析消息...");
        NetMessage receivedMessage = DeserializeMessage(requestBytes);
        
        if (receivedMessage != null && receivedMessage.Request != null)
        {
            if (receivedMessage.Request.playerJoinRoom != null)
            {
                Debug.Log("4. 成功接收到 PlayerJoinRoom 请求！");
                
                // 处理请求...
                
                // 创建响应
                Debug.Log("5. 创建响应消息...");
                byte[] responseBytes = CreatePlayerJoinRoomResponse();
                
                Debug.Log("6. 发送响应...");
                // 发送响应...
            }
        }
        
        Debug.Log("=== 示例完成 ===");
    }
    
    // Unity测试用
    void Start()
    {
        // 取消注释以运行示例
        // CompleteExample();
    }
}

