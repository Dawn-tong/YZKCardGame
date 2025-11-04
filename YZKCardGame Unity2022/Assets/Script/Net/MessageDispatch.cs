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
                Debug.LogWarning($"{Log.perfix}[消息分发] 请求消息为空");
                return;
            }
            if (message.playerJoinRoom != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.playerJoinRoom); }
            if (message.changeReady != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.changeReady); }
		}
        /// <summary>
        /// 分发响应消息
        /// </summary>
        public void Dispatch(T sender, NetMessageResponse message) {
            if (message == null) {
                Debug.LogWarning($"{Log.perfix}[消息分发] 响应消息为空");
                return;
            }
            if (message.PlayerJoinRoom != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.PlayerJoinRoom); }
            if (message.AddPlayerToRoom != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.AddPlayerToRoom); }
            if (message.changeReady != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.changeReady); }
            if (message.leaveRoom != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.leaveRoom); }
		}
    }
}