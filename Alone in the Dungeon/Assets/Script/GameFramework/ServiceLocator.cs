using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    /// <summary>
    /// 服务定位器 - 替代部分单例，实现依赖注入
    /// 提供服务查找，解耦服务与具体实现
    /// </summary>
    public class ServiceLocator : MonoBehaviour
    {
        public static ServiceLocator Instance { get; private set; }

        private Dictionary<Type, object> _services = new Dictionary<Type, object>();

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
        /// 注册服务
        /// </summary>
        public void Register<T>(T service) where T : class
        {
            Type type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service {type.Name} already registered, replacing...");
                _services[type] = service;
            }
            else
            {
                _services.Add(type, service);
            }
        }

        /// <summary>
        /// 注销服务
        /// </summary>
        public void Unregister<T>() where T : class
        {
            Type type = typeof(T);
            _services.Remove(type);
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        public T Get<T>() where T : class
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object service))
            {
                return service as T;
            }
            return null;
        }

        /// <summary>
        /// 尝试获取服务
        /// </summary>
        public bool TryGet<T>(out T service) where T : class
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object result))
            {
                service = result as T;
                return true;
            }
            service = null;
            return false;
        }
    }

    #region 服务接口定义

    /// <summary>
    /// 玩家服务接口 - 敌人通过此接口获取玩家信息
    /// </summary>
    public interface IPlayerProvider
    {
        Transform PlayerTransform { get; }
        GameObject PlayerGameObject { get; }
        bool IsPlayerAlive { get; }
    }

    /// <summary>
    /// 音频服务接口
    /// </summary>
    public interface IAudioService
    {
        void PlaySFX(int sfxIndex);
        void PlayMusic(AudioSource music);
        void StopMusic();
    }

    /// <summary>
    /// UI服务接口
    /// </summary>
    public interface IUIService
    {
        void UpdateHealthBar(float currentHealth, float maxHealth);
        void ShowGameOverPanel();
        void TogglePauseMenu();
    }

    #endregion
}
