using System;
using System.IO;
using UnityEngine;
using ProtoMessage;
using ProtoBuf;

/// <summary>
/// 消息打包处理器 - 负责消息的序列化和反序列化（使用ProtoBuf）
/// </summary>
public static class PackageHandler {
    /// <summary>
    /// 将消息打包成字节数组（使用ProtoBuf）
    /// </summary>
    public static byte[] PackMessage(NetMessage message) {
        try {
            using (MemoryStream stream = new MemoryStream()) {
                // 使用 ProtoBuf 序列化
                Serializer.Serialize(stream, message);
                byte[] data = stream.ToArray();

                //Debug.Log($"[ProtoBuf] 消息打包成功，大小: {data.Length} 字节");
                return data;  // 现在不添加长度前缀，由FastBufferWriter处理
            }
        }
        catch (Exception e) {
            Debug.LogError($"[ProtoBuf] 消息打包失败: {e.Message}\n{e.StackTrace}");
            return null;
        }
    }
    
    /// <summary>
    /// 将字节数组解包成消息（使用ProtoBuf）
    /// </summary>
    public static NetMessage UnpackMessage(byte[] data, int offset = 0, int length = -1) {
        try {
            if (length == -1) {
                length = data.Length - offset;
            }
            
            // 直接使用ProtoBuf反序列化，因为长度前缀已经在FastBufferReader中处理了
            using (MemoryStream stream = new MemoryStream(data, offset, length)) {
                NetMessage message = Serializer.Deserialize<NetMessage>(stream);
                return message;
            }
        }
        catch (Exception e) {
            Debug.LogError($"[ProtoBuf] 消息解包失败: {e.Message}\n{e.StackTrace}");
            return null;
        }
    }
    
    /// <summary>
    /// 从流中读取消息（使用ProtoBuf）
    /// </summary>
    /// <param playerName="stream">数据流</param>
    /// <returns>解包后的消息</returns>
    public static NetMessage ReadMessageFromStream(Stream stream) {
        try {
            // 读取消息长度
            byte[] lengthBuffer = new byte[4];
            int bytesRead = stream.Read(lengthBuffer, 0, 4);
            if (bytesRead != 4) {
                Debug.LogWarning("[ProtoBuf] 无法读取消息长度");
                return null;
            }
            
            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
            
            // 读取消息内容
            byte[] messageBuffer = new byte[messageLength];
            bytesRead = stream.Read(messageBuffer, 0, messageLength);
            if (bytesRead != messageLength) {
                Debug.LogWarning($"[ProtoBuf] 消息不完整: 期望{messageLength}字节，实际读取{bytesRead}字节");
                return null;
            }
            
            // 使用 ProtoBuf 反序列化
            using (MemoryStream messageStream = new MemoryStream(messageBuffer)) {
                NetMessage message = Serializer.Deserialize<NetMessage>(messageStream);
                return message;
            }
        }
        catch (Exception e) {
            Debug.LogError($"[ProtoBuf] 从流中读取消息失败: {e.Message}\n{e.StackTrace}");
            return null;
        }
    }
}