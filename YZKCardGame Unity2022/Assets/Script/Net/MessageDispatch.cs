using UnityEngine;
using ProtoMessage;

namespace Network {
	/// <summary>
	/// 消息分发器 - 自动检查消息字段并分发到对应的处理器（ProtoBuf版本）
	/// </summary>
	/// <typeparam playerName="T">发送者类型</typeparam>
	public class MessageDispatch<T> {
		static MessageDispatch<T> instance;
		public static MessageDispatch<T> Instance {
			get {
				if (instance == null) {
					instance = new MessageDispatch<T>();
				}
				return instance;
			}
		}
		
		
		
		/// <summary>
		/// 分发请求消息
		/// </summary>
		public void Dispatch(T sender, NetMessageRequest message) {
			if (message == null) {
				Debug.LogWarning($"{Log.perfix}MessageDispatch.请求消息为空");
				return;
			}
			bool success = false;
			if (message.playerJoinRoom != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.playerJoinRoom); success = true; }
			if (message.changeReady != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.changeReady); success = true; }
			if (message.playerJoinGame != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.playerJoinGame); success = true; }
			if (message.turnAction != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.turnAction); success = true; }
			if (!success) { 
                Debug.LogWarning($"{Log.perfix}MessageDispatch.请求消息未分发"); 
                UIMessagePanel.Instance.AddMessage($"请求消息未分发");    
            }
		}
		/// <summary>
		/// 分发响应消息
		/// </summary>
		public void Dispatch(T sender, NetMessageResponse message) {
			if (message == null) {
				Debug.LogWarning($"{Log.perfix}MessageDispatch.响应消息为空");
				return;
			}
			bool success = false;
			if (message.PlayerJoinRoom != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.PlayerJoinRoom); success = true; }
			if (message.AddPlayerToRoom != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.AddPlayerToRoom); success = true; }
			if (message.changeReady != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.changeReady); success = true; }
			if (message.leaveRoom != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.leaveRoom); success = true; }
			if (message.readyToStart != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.readyToStart); success = true; }
			if (message.failedToJoinGame != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.failedToJoinGame); success = true; }
			if (message.gameStart != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.gameStart); success = true; }
			if (message.turnToPlay != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.turnToPlay); success = true; }
			if (message.playerAction != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.playerAction); success = true; }
			if (!success) { 
                Debug.LogWarning($"{Log.perfix}MessageDispatch.响应消息未分发"); 
                UIMessagePanel.Instance.AddMessage($"响应消息未分发");    
            }
		}
	}
}