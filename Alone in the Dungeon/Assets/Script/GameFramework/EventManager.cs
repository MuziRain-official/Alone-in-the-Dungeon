using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    /// <summary>
    /// 全局事件中心 - 解耦系统间通信
    /// 所有模块通过事件进行交互，避免直接引用
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        private Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

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
        /// 订阅事件
        /// </summary>
        public void Subscribe<T>(Action<T> handler) where T : struct
        {
            Type eventType = typeof(T);
            if (_events.ContainsKey(eventType))
            {
                _events[eventType] = Delegate.Combine(_events[eventType], handler);
            }
            else
            {
                _events[eventType] = handler;
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

    #region 游戏事件定义

    /// <summary>
    /// 玩家受伤事件
    /// </summary>
    public struct PlayerDamageEvent
    {
        public float damage;
        public float currentHealth;
        public float maxHealth;
    }

    /// <summary>
    /// 玩家死亡事件
    /// </summary>
    public struct PlayerDeathEvent { }

    /// <summary>
    /// 玩家治愈事件
    /// </summary>
    public struct PlayerHealEvent
    {
        public float healAmount;
        public float currentHealth;
        public float maxHealth;
    }

    /// <summary>
    /// 敌人死亡事件
    /// </summary>
    public struct EnemyDeathEvent
    {
        public GameObject enemy;
    }

    /// <summary>
    /// 房间清理完成事件
    /// </summary>
    public struct RoomClearedEvent
    {
        public GameObject room;
    }

    /// <summary>
    /// 玩家进入房间事件
    /// </summary>
    public struct PlayerEnterRoomEvent
    {
        public GameObject room;
    }

    /// <summary>
    /// 游戏暂停事件
    /// </summary>
    public struct GamePauseEvent
    {
        public bool isPaused;
    }

    /// <summary>
    /// Boss激活事件
    /// </summary>
    public struct BossActivationEvent
    {
        public GameObject boss;
        public EnemyController.EnemyHealth bossHealth;
    }

    #endregion
}
