using System;
using UnityEngine;

namespace PlayerController
{
    /// <summary>
    /// 玩家生命值管理 - 实现 IDamageable 和 IHealable 接口
    /// </summary>
    public class PlayerHealth : MonoBehaviour, GameFramework.IDamageable, GameFramework.IHealable
    {
        public static PlayerHealth Instance { get; private set; }

        [Header("生命值设置")]
        public float maxHealth = 100f;
        public float currentHealth;

        // IDamageable 接口实现
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => currentHealth > 0;

        public event Action<float> OnDamaged;
        public event Action OnDied;
        public event Action<float> OnHealed;

        // 兼容旧事件（不推荐使用）
        public event Action<float> OnDamage;
        public event Action<float> OnHeal;

        private DashSkill dashSkill;
        private GameFramework.EventManager eventManager;

        void Awake()
        {
            Instance = this;

            // 通过服务定位器注册玩家服务（在 Awake 中注册，确保敌人初始化时能找到）
            if (GameFramework.ServiceLocator.Instance != null)
            {
                GameFramework.ServiceLocator.Instance.Register(new PlayerProvider(this));
            }
        }

        void Start()
        {
            dashSkill = GetComponent<DashSkill>();
            currentHealth = maxHealth;
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            // 注销服务
            if (GameFramework.ServiceLocator.Instance != null)
            {
                GameFramework.ServiceLocator.Instance.Unregister<GameFramework.IPlayerProvider>();
            }
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            if (dashSkill != null && dashSkill.IsInvincible)
            {
                return; // 无敌状态
            }

            currentHealth -= damage;

            // 播放受伤音效 - 优先使用服务定位器，备用直接调用
            var audioService = GameFramework.ServiceLocator.Instance?.Get<GameFramework.IAudioService>();
            if (audioService != null)
            {
                audioService.PlaySFX(1); // 受伤音效
            }
            else if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(1);
            }

            OnDamaged?.Invoke(damage);
            OnDamage?.Invoke(damage);

            // 通过事件中心发布受伤事件
            eventManager = GameFramework.EventManager.Instance;
            eventManager?.Publish(new GameFramework.PlayerDamageEvent
            {
                damage = damage,
                currentHealth = currentHealth,
                maxHealth = maxHealth
            });

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }

        void Die()
        {
            OnDied?.Invoke();

            // 播放死亡音效
            var audioService = GameFramework.ServiceLocator.Instance?.Get<GameFramework.IAudioService>();
            if (audioService != null)
            {
                audioService.PlaySFX(6); // 死亡音效
            }
            else if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(6);
            }

            // 通过事件中心发布死亡事件
            eventManager?.Publish(new GameFramework.PlayerDeathEvent());

            // 延迟销毁，让订阅者有时间响应
            Destroy(gameObject, 0.1f);
        }

        public void Heal(int healAmount)
        {
            if (!IsAlive) return;

            currentHealth += healAmount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            // 播放治愈音效
            var audioService = GameFramework.ServiceLocator.Instance?.Get<GameFramework.IAudioService>();
            if (audioService != null)
            {
                audioService.PlaySFX(2); // 治愈音效
            }
            else if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(2);
            }

            OnHealed?.Invoke(healAmount);
            OnHeal?.Invoke(healAmount);

            // 通过事件中心发布治愈事件
            eventManager = GameFramework.EventManager.Instance;
            eventManager?.Publish(new GameFramework.PlayerHealEvent
            {
                healAmount = healAmount,
                currentHealth = currentHealth,
                maxHealth = maxHealth
            });
        }

        /// <summary>
        /// 玩家服务实现 - 供敌人系统通过服务定位器获取玩家信息
        /// </summary>
        private class PlayerProvider : GameFramework.IPlayerProvider
        {
            private PlayerHealth playerHealth;

            public PlayerProvider(PlayerHealth playerHealth)
            {
                this.playerHealth = playerHealth;
            }

            public Transform PlayerTransform => playerHealth != null ? playerHealth.transform : null;
            public GameObject PlayerGameObject => playerHealth != null ? playerHealth.gameObject : null;
            public bool IsPlayerAlive => playerHealth != null && playerHealth.IsAlive;
        }
    }
}
