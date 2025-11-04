========================================
消息系统使用指南
========================================

## 快速开始

### 1. 订阅消息
```csharp
void Start()
{
    NetManager.Instance.Subscribe<MapCharacterLeaveResponse>(OnCharacterLeave);
}

void OnCharacterLeave(ulong senderId, MapCharacterLeaveResponse message)
{
    Debug.Log($"角色 {message.characterDBID} 离开");
}
```

### 2. 发送消息给所有玩家
```csharp
NetMessage message = new NetMessage();
message.Response = new NetMessageResponse();
message.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
message.Response.mapCharacterLeave.characterDBID = 12345;

NetManager.Instance.SendMessageToAll(message);
```

### 3. 发送消息给指定玩家
```csharp
NetManager.Instance.SendMessageToPlayer(message, targetPlayerId);
```

### 4. 取消订阅
```csharp
void OnDestroy()
{
    NetManager.Instance.Unsubscribe<MapCharacterLeaveResponse>(OnCharacterLeave);
}
```

## 添加新消息类型

### 步骤1：定义消息类
在 NetMessageResponse.cs 或 NetMessageRequest.cs 中添加：
```csharp
[Serializable]
public class CustomActionResponse
{
    public int actionId;
}

public class NetMessageResponse
{
    public CustomActionResponse customAction;
}
```

### 步骤2：添加分发逻辑
在 MessageDispatch.cs 的 Dispatch 方法中添加：
```csharp
if (message.customAction != null)
{
    MessageDistributer<T>.Instance.RaiseEvent(sender, message.customAction);
}
```

### 步骤3：订阅和使用
```csharp
NetManager.Instance.Subscribe<CustomActionResponse>(OnCustomAction);

void OnCustomAction(ulong senderId, CustomActionResponse message)
{
    Debug.Log($"自定义动作: {message.actionId}");
}
```

## 核心文件
- MessageDistributer.cs - 订阅管理
- MessageDispatch.cs - 消息分发
- NetMessage.cs - 主消息类
- NetMessageRequest.cs - 请求消息
- NetMessageResponse.cs - 响应消息
- PackageHandler.cs - 序列化/反序列化

详细说明请参考: 新使用说明.txt

