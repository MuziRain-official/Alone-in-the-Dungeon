using System;
using UnityEngine;

namespace GameFramework
{
    /// <summary>
    /// 可伤害接口 - 统一处理玩家和敌人的受伤逻辑
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// 受到伤害
        /// </summary>
        void TakeDamage(int damage);

        /// <summary>
        /// 当前生命值
        /// </summary>
        float CurrentHealth { get; }

        /// <summary>
        /// 最大生命值
        /// </summary>
        float MaxHealth { get; }

        /// <summary>
        /// 是否存活
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// 受伤事件
        /// </summary>
        event Action<float> OnDamaged;

        /// <summary>
        /// 死亡事件
        /// </summary>
        event Action OnDied;
    }

    /// <summary>
    /// 可治愈接口 - 药水等治疗物体使用
    /// </summary>
    public interface IHealable
    {
        void Heal(int healAmount);
        event Action<float> OnHealed;
    }
}
