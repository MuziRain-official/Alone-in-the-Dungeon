using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    /// <summary>
    /// 全局事件中心 - 解耦系统间通信
    /// 所有模块通过事件进行交互，避免直接引用
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        private Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();
        private HashSet<Type> _registeredEvents = new HashSet<Type>();
        private Dictionary<Type, List<Delegate>> _pendingSubscriptions = new Dictionary<Type, List<Delegate>>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 注册事件类型（由事件所属模块调用）
        /// </summary>
        public void Register<T>() where T : struct
        {
            Type t = typeof(T);
            if (_registeredEvents.Add(t))
            {
                if (!_events.ContainsKey(t))
                    _events[t] = null;

                // 刷新该事件的延迟订阅
                if (_pendingSubscriptions.TryGetValue(t, out var pending))
                {
                    foreach (var handler in pending)
                        _events[t] = Delegate.Combine(_events[t], handler);
                    _pendingSubscriptions.Remove(t);
                }
            }
        }

        /// <summary>
        /// 查询事件是否已注册
        /// </summary>
        public bool IsRegistered<T>() where T : struct
        {
            return _registeredEvents.Contains(typeof(T));
        }

        /// <summary>
        /// 订阅事件（若事件尚未注册则延迟绑定）
        /// </summary>
        public void Subscribe<T>(Action<T> handler) where T : struct
        {
            Type t = typeof(T);
            if (_registeredEvents.Contains(t))
            {
                if (!_events.ContainsKey(t))
                    _events[t] = null;
                _events[t] = Delegate.Combine(_events[t], handler);
            }
            else
            {
                if (!_pendingSubscriptions.ContainsKey(t))
                    _pendingSubscriptions[t] = new List<Delegate>();
                _pendingSubscriptions[t].Add(handler);
            }
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        public void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            Type eventType = typeof(T);
            if (_events.ContainsKey(eventType))
            {
                _events[eventType] = Delegate.Remove(_events[eventType], handler);
            }
        }

        /// <summary>
        /// 清空所有状态（用于游戏重新开始时）
        /// </summary>
        public void ClearAllEvents()
        {
            _events.Clear();
            _registeredEvents.Clear();
            _pendingSubscriptions.Clear();
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public void Publish<T>(T eventData) where T : struct
        {
            Type eventType = typeof(T);
            if (_events.ContainsKey(eventType) && _events[eventType] != null)
            {
                ((Action<T>)_events[eventType])?.Invoke(eventData);
            }
        }
    }

    #region 框架级事件定义

    /// <summary>
    /// 游戏暂停事件（框架级，保留在核心层）
    /// </summary>
    public struct GamePauseEvent
    {
        public bool isPaused;
    }

    #endregion
}
