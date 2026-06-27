using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    /// <summary>
    /// 游戏模块生命周期接口
    /// 分两阶段初始化：先注册事件 → 再订阅事件
    /// </summary>
    public interface IGameModule
    {
        /// <summary>注册本模块产生的事件（不依赖其他模块）</summary>
        void RegisterEvents();

        /// <summary>订阅其他模块的事件（此时所有事件均已注册）</summary>
        void SubscribeEvents();
    }

    /// <summary>
    /// 生命周期管理器 - 保证模块初始化顺序
    /// 首次调用 RegisterModule 时自动创建实例
    /// 在 Start 阶段按序执行 RegisterEvents → SubscribeEvents
    /// </summary>
    [DefaultExecutionOrder(-90)]
    public class LifecycleManager : MonoBehaviour
    {
        public static LifecycleManager Instance { get; private set; }
        private List<IGameModule> _modules = new List<IGameModule>();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            foreach (var module in _modules)
                module.RegisterEvents();
            foreach (var module in _modules)
                module.SubscribeEvents();
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// 注册模块（由模块在 Awake 中调用）
        /// 首次调用时自动创建 LifecycleManager 实例
        /// </summary>
        public static void RegisterModule(IGameModule module)
        {
            if (Instance == null)
            {
                var go = new GameObject("LifecycleManager");
                go.AddComponent<LifecycleManager>(); // Awake 中会设置 Instance
            }
            Instance._modules.Add(module);
        }
    }
}
