using Network;
using ProtoMessage;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;

public class NetManager : ManagerBase<NetManager> {
	// 标记消息处理器是否已注册
	private bool isMessageHandlerRegistered = false;
    public void Init() {
        EnsureNetworkComponents();	//创建网络组件
        InitializeNetworkServices();
        RegisterNetworkCallbacks();	//监听网络断开事件
    }
	// 自动创建所需的网络组件
	void EnsureNetworkComponents() {
		// 检查是否已存在 Unity 的 NetworkManager(只会运行一次,必定不存在)
		// if (NetworkManager.Singleton == null) {
		Debug.Log("自动创建 NetworkManager 组件...");
		// 创建 GameObject 并添加必要组件
		var networkObject = new GameObject("NetworkManager");
		DontDestroyOnLoad(networkObject);
		//此组件无法放在其他节点下
		//networkObject.transform.SetParent(ManagerObj.transform);
		var unityTransport = networkObject.AddComponent<UnityTransport>();
		var unityNetworkManager = networkObject.AddComponent<NetworkManager>();
		unityNetworkManager.NetworkConfig = new NetworkConfig() {
			NetworkTransport = unityTransport,  // 设置传输组件
			ProtocolVersion = 1  // 设置协议版本
		};
		//Debug.Log("NetworkManager 和 UnityTransport 组件创建完成");
	}
	async void InitializeNetworkServices() {
		try {
			if (UnityServices.State != ServicesInitializationState.Initialized) {
				await UnityServices.InitializeAsync();
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
				string playerName = AuthenticationService.Instance.PlayerId.Substring(0, 16);
				PlayerManager.Instance.currentPlayer.SetPlayerName(playerName);
			}
		}
		catch (System.Exception e) {
			Debug.LogError($"网络服务初始化失败: {e.Message}");
			UIMessagePanel.Instance.AddMessage($"网络服务初始化失败: {e.Message}");
		}
		GameManager.FinishInit();
	}
	// 应用程序退出时清理
	void OnApplicationQuit()
	{
		UnregisterMessageHandler();
		UnregisterNetworkCallbacks();
		if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient)
		{
			Debug.Log("应用程序退出，清理网络资源");
			LeaveRoom();
		}
	}






	public bool isHostPlayer;	//用于记录点击的是创建房间还是加入游戏
	private string currentJoinCode;
	public delegate void ConnectDelegate();
	public event ConnectDelegate OnCreatRoomSuccess;
	public event ConnectDelegate OnJoinRoomSuccess;
	public event ConnectDelegate OnJoinRoomFailed;
	public string GetCurrentJoinCode() => currentJoinCode;
	// 创建Relay房间并启动主机
	public async void CreateRelayRoom(int maxPlayers = 4) {
		Debug.Log($"{Log.perfix}正在创建房间...");
		try {
			// 1. 创建Relay分配
			var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
			currentJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
			// 2. 配置网络传输
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
				allocation.RelayServer.IpV4,
				(ushort)allocation.RelayServer.Port,
				allocation.AllocationIdBytes,
				allocation.Key,
				allocation.ConnectionData
			);
			// 3. 启动主机 - 添加详细日志
			bool hostStarted = NetworkManager.Singleton.StartHost();
			//Debug.Log($"StartHost() 返回值: {hostStarted}");
			if (hostStarted) {
				// 等待一帧让网络状态更新
				await System.Threading.Tasks.Task.Delay(100);
				//Debug.Log($"网络状态 - IsServer: {NetworkManager.Singleton.IsServer}, IsHost: {NetworkManager.Singleton.IsHost}");

				// 网络启动后注册消息处理器
				RegisterMessageHandler();
				PlayerManager.Instance.currentPlayer.SetNetID(GetLocalClientId());
				Debug.Log($"房间创建成功！加入码 = {currentJoinCode}; seatID = {GetLocalClientId()}");
				UIMessagePanel.Instance.AddMessage($"房间创建成功！将加入码分享给朋友即可加入房间");
				OnCreatRoomSuccess?.Invoke();
			}
			else {
				Debug.LogError("StartHost() 启动失败！");
				UIMessagePanel.Instance.AddMessage("房间创建失败，请重试");
				// 回退到大厅场景
				SceneLoaderManager.Instance.LoadScene("HallScene");
			}
		}
		catch (System.Exception e) {
			Debug.LogError($"创建房间失败: {e.Message}");
			UIMessagePanel.Instance.AddMessage($"创建房间失败: {e.Message}");
			SceneLoaderManager.Instance.LoadScene("HallScene");
		}
	}
	// 加入房间
	public async void JoinRelayRoom(string joinCode) {
		if (string.IsNullOrEmpty(joinCode)) {
			Debug.LogError("加入码不能为空");
			UIMessagePanel.Instance.AddMessage($"加入码不能为空");
			return;
		}
		currentJoinCode = joinCode;
		Debug.Log($"正在加入房间: {joinCode}");
		try {
			// 1. 通过加入码获取Relay分配信息
			var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
			// 2. 配置客户端网络传输
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
				joinAllocation.RelayServer.IpV4,
				(ushort)joinAllocation.RelayServer.Port,
				joinAllocation.AllocationIdBytes,
				joinAllocation.Key,
				joinAllocation.ConnectionData,
				joinAllocation.HostConnectionData
			);
			// 3. 注册连接成功回调
			NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
			// 4. 启动客户端连接
			if (NetworkManager.Singleton.StartClient()) {
				RegisterMessageHandler();
				Debug.Log("客户端启动完成，等待连接回调...");
			}
		}
		catch (System.Exception e) {
			Debug.LogError($"加入房间失败: {e.Message}");
			OnJoinRoomFailed?.Invoke();
			// ... 错误处理
		}
	}
	// 客户端连接成功回调
	void OnClientConnected(ulong clientId) {
		// 确保是本地客户端连接成功
		if (clientId == NetworkManager.Singleton.LocalClientId) {
			Debug.Log($"客户端连接成功回调！ClientId: {clientId}, IsConnected: {NetworkManager.Singleton.IsConnectedClient}");
			// 取消注册回调，避免重复触发
			NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
			// 触发连接成功事件
			OnJoinRoomSuccess?.Invoke();
		}
	}
	//注册消息处理器
	void RegisterMessageHandler() {
		if (isMessageHandlerRegistered) {
			UIMessagePanel.Instance.AddMessage("消息处理器已经注册过了，跳过重复注册");
			Debug.LogWarning("消息处理器已经注册过了，跳过重复注册");
			return;
		}
		if (NetworkManager.Singleton != null && NetworkManager.Singleton.CustomMessagingManager != null) {
			// 注册自定义消息处理器
			NetworkManager.Singleton.CustomMessagingManager.OnUnnamedMessage += HandleIncomingMessage;
			isMessageHandlerRegistered = true;
			UIMessagePanel.Instance.AddMessage("消息处理器注册完成");
			Debug.Log("消息处理器注册完成");
		}
		else {
			Debug.LogError("无法注册消息处理器：NetworkManager 或 CustomMessagingManager 为空");
		}
	}
	// 离开房间
	public void LeaveRoom() {
		currentJoinCode = null;
		// 取消注册消息处理器
		UnregisterMessageHandler();
		if (NetworkManager.Singleton.IsClient) {
			NetworkManager.Singleton.Shutdown();
		}
	}
	//取消注册消息处理器
	void UnregisterMessageHandler() {
		if (!isMessageHandlerRegistered) {
			return;
		}
		isMessageHandlerRegistered = false;

		if (NetworkManager.Singleton != null && NetworkManager.Singleton.CustomMessagingManager != null) {
			NetworkManager.Singleton.CustomMessagingManager.OnUnnamedMessage -= HandleIncomingMessage;
		}
		UIMessagePanel.Instance.AddMessage("消息处理器已取消注册");
		Debug.Log($"{Log.perfix}消息处理器已取消注册");
	}






	// ==================== 网络掉线处理 ====================
	public delegate void NetDisconnectedDelegate(ulong clientID);
	public event NetDisconnectedDelegate NetDisconnected;
	//注册网络断开回调
	void RegisterNetworkCallbacks() {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkClientDisconnected;
        }
    }
	void UnregisterNetworkCallbacks() {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnNetworkClientDisconnected;
        }
    }
	//收到网络断开消息(收到退出响应以后也会收到网络断开消息)
	void OnNetworkClientDisconnected(ulong clientID) {
		// 服务器掉线/主动退出房间(即客户端发现已经断开连接)
		if (clientID == PlayerManager.Instance.currentPlayer.netID) {
			Log.IncreasePerfixLength();
			Debug.Log($"{Log.perfix}————        网络已断开        ————");
			UIMessagePanel.Instance.AddMessage("网络已断开");
			Log.ReducePerfixLength();
			NetDisconnected?.Invoke(clientID);
			return;
		}
		Debug.Log($"{Log.perfix}NetID={clientID}断开网络连接");
		UIMessagePanel.Instance.AddMessage($"NetID={clientID}断开网络连接");
		//服务器发现有玩家掉线
		if (NetworkManager.Singleton.IsServer) {
			NetDisconnected?.Invoke(clientID);
		}
    }





	// ==================== 消息发送和接收功能 ====================
	public ulong GetLocalClientId() => NetworkManager.Singleton.LocalClientId;
	/// <summary>
	/// 发送消息给所有玩家
	/// </summary>
	/// <param playerName="message">要发送的消息</param>
	/// <param playerName="excludeSelf">是否排除自己</param>
	public void SendMessageToAll(NetMessage message, ulong? expectID = null, bool excludeSelf = true) {
		if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) {
			Debug.LogWarning($"{Log.perfix}只有服务器/主机才能发送消息给所有玩家");
			return;
		}
		try {
			// 打包消息
			byte[] data = PackageHandler.PackMessage(message);
			if (data == null || data.Length == 0) {
				Debug.LogError($"{Log.perfix}消息打包失败");
				return;
			}
			// 发送给所有客户端
			foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds) {
				if (excludeSelf && clientId == NetworkManager.Singleton.LocalClientId) {
					continue;
				}
				if (expectID != null && clientId == expectID) {
					continue;
				}
				SendDataToClient(clientId, data);
			}
		}
		catch (Exception e) {
			Debug.LogError($"{Log.perfix}发送消息给所有玩家失败: {e.Message}");
		}
	}
	/// <summary>
	/// 发送消息给指定玩家
	/// </summary>
	public void SendMessageToPlayer(ulong targetClientId, NetMessage message) {
		if (NetworkManager.Singleton == null) {
			Debug.LogWarning($"{Log.perfix}网络管理器未初始化");
			return;
		}
		if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient) {
			Debug.LogWarning($"{Log.perfix}未连接到网络");
			return;
		}
		try {
			// 打包消息
			byte[] data = PackageHandler.PackMessage(message);
			if (data == null || data.Length == 0) {
				Debug.LogError($"{Log.perfix}消息打包失败");
				return;
			}
			// 发送给指定客户端
			SendDataToClient(targetClientId, data);
		}
		catch (Exception e) {
			Debug.LogError($"{Log.perfix}发送消息给玩家失败: {e.Message}");
		}
	}
	/// <summary>
	/// 发送消息给主机玩家/服务器
	/// </summary>
	/// <param playerName="message">要发送的消息</param>
	public void SendMessageToServer(NetMessage message) {
		if (NetworkManager.Singleton == null) {
			Debug.LogWarning($"{Log.perfix}网络管理器未初始化");
			return;
		}
		if (!NetworkManager.Singleton.IsClient) {
			Debug.LogWarning($"{Log.perfix}未连接到网络");
			return;
		}
		if (NetworkManager.Singleton.IsServer) {
			Debug.LogWarning($"{Log.perfix}服务器无需给自己发送消息");
			return;
		}
		try {
			// 打包消息
			byte[] data = PackageHandler.PackMessage(message);
			if (data == null || data.Length == 0) {
				Debug.LogError($"{Log.perfix}消息打包失败");
				return;
			}
			// 发送给服务器
			SendDataToClient(NetworkManager.ServerClientId, data);
		}
		catch (Exception e) {
			Debug.LogError($"{Log.perfix}发送消息给服务器失败: {e.Message}");
		}
	}
	/// <summary>
	/// 发送数据给指定客户端
	/// </summary>
	void SendDataToClient(ulong clientId, byte[] data) {
	    if (data == null || data.Length == 0) {
	        Debug.LogError($"{Log.perfix}发送的数据为空");
	        return;
	    }

	    // 计算需要的总空间：数据长度 + 长度前缀(4字节)
	    int totalSize = data.Length + sizeof(int);
	    var writer = new FastBufferWriter(totalSize, Unity.Collections.Allocator.Temp);
	
	    try {
	        // 必须先调用 TryBeginWrite 来分配写入空间
	        if (!writer.TryBeginWrite(totalSize)) {
	            Debug.LogError($"{Log.perfix}无法分配足够的写入空间: {totalSize} 字节");
	            return;
	        }

	        // 使用 WriteValueSafe 确保安全写入
	        writer.WriteValueSafe(data.Length);  // 先写入数据长度
	        writer.WriteBytesSafe(data, data.Length); // 使用WriteBytesSafe写入实际数据

	        if (NetworkManager.Singleton.IsServer) {
	            NetworkManager.Singleton.CustomMessagingManager.SendUnnamedMessage(
	                clientId,
	                writer,
	                NetworkDelivery.ReliableFragmentedSequenced
	            );
	            Debug.Log($"{Log.perfix}服务器发送消息给客户端 {clientId}。长度 = {data.Length}, 总写入 = {writer.Length} 字节");
	        }
	        else if (NetworkManager.Singleton.IsClient) {
	            NetworkManager.Singleton.CustomMessagingManager.SendUnnamedMessage(
	                NetworkManager.ServerClientId,
	                writer,
	                NetworkDelivery.ReliableFragmentedSequenced
	            );
	            Debug.Log($"{Log.perfix}客户端发送消息给服务器。长度 = {data.Length}, 总写入 = {writer.Length} 字节");
	        }
	    }
	    catch (Exception e) {
	        Debug.LogError($"{Log.perfix}发送消息失败: {e.Message}\n{e.StackTrace}");
	    }
	    finally {
	        writer.Dispose();
	    }
	}
	/// <summary>
	/// 处理接收到的消息
	/// </summary>
	void HandleIncomingMessage(ulong senderId, FastBufferReader reader) {
	    try {
	        // 读取数据长度
	        if (!reader.TryBeginRead(sizeof(int))) {
	            Debug.LogError("无法读取数据长度");
	            return;
	        }
	        reader.ReadValueSafe(out int dataLength);
			// 检查是否有足够的数据可读
	        if (!reader.TryBeginRead(dataLength)) {
	            Debug.LogError($"数据不完整，期望 {dataLength} 字节");
	            return;
	        }
	        // 读取实际数据
	        byte[] data = new byte[dataLength];
	        reader.ReadBytesSafe(ref data, dataLength);
	        // 解包消息
	        NetMessage message = PackageHandler.UnpackMessage(data);
	        if (message == null) {
	            Debug.LogError("消息解包失败");
	            return;
	        }
			// 使用消息分发系统
			if (isHostPlayer) {
				//服务器只处理请求
				if (message.Request != null) {
			        Log.IncreasePerfixLength();
					Debug.Log($"{Log.perfix}————        服务器收到一条请求(来源ID:{senderId})        ————");
					MessageDispatch<ulong>.Instance.Dispatch(senderId, message.Request);
					Log.ReducePerfixLength();
				}
			}
			else {
				//客户端只处理响应
				if (message.Response != null) {
			        Log.IncreasePerfixLength();
					Debug.Log($"{Log.perfix}————        客户端收到一条响应        ————");
					MessageDispatch<ulong>.Instance.Dispatch(senderId, message.Response);
					Log.ReducePerfixLength();
				}
			}
	    }
	    catch (Exception e) {
	        Debug.LogError($"处理接收消息失败: {e.Message}\n{e.StackTrace}");
	    }
	}
}