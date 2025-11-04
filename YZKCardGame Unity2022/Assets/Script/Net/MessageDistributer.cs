using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network {
    /// <summary>
    /// 消息分发者 - 基于泛型的事件系统
    /// </summary>
    /// <typeparam playerName="T">发送者类型</typeparam>
    public class MessageDistributer<T> {
        private static MessageDistributer<T> instance;
        public static MessageDistributer<T> Instance {
            get {
                if (instance == null) {
                    instance = new MessageDistributer<T>();
                }
                return instance;
            }
        }

        // 存储所有消息类型的委托
        private Dictionary<Type, Delegate> messageHandlers = new Dictionary<Type, Delegate>();
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam playerName="TMessage">消息类型</typeparam>
        /// <param playerName="handler">处理器委托</param>
        public void Subscribe<TMessage>(Action<T, TMessage> handler) where TMessage : class {
            Type messageType = typeof(TMessage);
            if (messageHandlers.ContainsKey(messageType)) {
                messageHandlers[messageType] = Delegate.Combine(messageHandlers[messageType], handler);
            }
            else {
                messageHandlers[messageType] = handler;
            }
            Debug.Log($"{Log.perfix}[消息订阅] 订阅消息类型: {messageType.Name}");
        }
		/// <summary>
		/// 取消订阅消息
		/// </summary>
		/// <typeparam playerName="TMessage">消息类型</typeparam>
		/// <param playerName="handler">处理器委托</param>
		public void Unsubscribe<TMessage>(Action<T, TMessage> handler) where TMessage : class {
            Type messageType = typeof(TMessage);
            if (messageHandlers.ContainsKey(messageType)) {
                messageHandlers[messageType] = Delegate.Remove(messageHandlers[messageType], handler);
                // 如果没有订阅者了，移除这个键
                if (messageHandlers[messageType] == null) {
                    messageHandlers.Remove(messageType);
                }
                Debug.Log($"{Log.perfix}[消息订阅] 取消订阅消息类型: {messageType.Name}");
            }
        }
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam playerName="TMessage">消息类型</typeparam>
        /// <param playerName="sender">发送者</param>
        /// <param playerName="message">消息</param>
        public void RaiseEvent<TMessage>(T sender, TMessage message) where TMessage : class {
            Type messageType = typeof(TMessage);

            if (messageHandlers.ContainsKey(messageType)) {
                Debug.Log($"{Log.perfix}消息类型: {messageType.Name}");
                try {
                    var handler = messageHandlers[messageType] as Action<T, TMessage>;
                    handler?.Invoke(sender, message);
                }
                catch (Exception e) {
                    Debug.LogError($"{Log.perfix}[消息分发]处理消息 {messageType.Name} 时出错: {e.Message}\n{e.StackTrace}");
                }
            }
            else {
                Debug.LogWarning($"{Log.perfix}[消息分发]没有订阅者处理消息类型: {messageType.Name}");
            }
        }

        /// <summary>
        /// 清除所有订阅
        /// </summary>
        public void ClearAllSubscriptions() {
            messageHandlers.Clear();
            Debug.Log("[消息订阅] 已清除所有订阅");
        }
        /// <summary>
        /// 获取订阅数量
        /// </summary>
        public int GetSubscriptionCount() {
            return messageHandlers.Count;
        }
    }
}


