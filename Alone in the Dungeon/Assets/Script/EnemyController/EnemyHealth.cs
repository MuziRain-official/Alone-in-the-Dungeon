using System;
using UnityEngine;

namespace EnemyController
{
    /// <summary>
    /// 敌人生命值管理 - 实现 IDamageable 接口
    /// </summary>
    public class EnemyHealth : MonoBehaviour, GameFramework.IDamageable
    {
        [Header("生命值设置")]
        public int maxHealth = 20;
        public int currentHealth;

        // IDamageable 接口实现
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => currentHealth > 0;

        // 事件
        public event Action<float> OnDamaged;
        public event Action OnDied;

        // 兼容旧事件
        public event Action OnHurt;

        // 是否是Boss
        private bool isBoss;

        void Start()
        {
            currentHealth = maxHealth;
            // 检查是否是Boss
            isBoss = GetComponent<BossLogic>() != null;
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            currentHealth -= damage;
            OnDamaged?.Invoke(damage);
            OnHurt?.Invoke();

            // 播放音效 - 优先使用服务定位器，备用单例
            var audioService = GameFramework.ServiceLocator.Instance?.Get<GameFramework.IAudioService>();
            if (audioService != null)
            {
                audioService.PlaySFX(3);
            }
            else if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(3);
            }

            // 只有Boss才发布受伤事件（用于更新Boss血条）
            if (isBoss)
            {
                GameFramework.EventManager.Instance?.Publish(new GameFramework.BossDamageEvent
                {
                    boss = gameObject,
                    currentHealth = currentHealth,
                    maxHealth = maxHealth
                });
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }

        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        }

        private void Die()
        {
            OnDied?.Invoke();

            // 通过事件中心发布死亡事件
            if (GameFramework.EventManager.Instance != null)
            {
                GameFramework.EventManager.Instance.Publish(new GameFramework.EnemyDeathEvent { enemy = gameObject });
            }

            // 延迟销毁，让订阅者有时间响应
            Destroy(gameObject, 0.1f);
        }
    }
}
