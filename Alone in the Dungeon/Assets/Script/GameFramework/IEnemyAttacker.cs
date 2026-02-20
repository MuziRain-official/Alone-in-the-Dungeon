using System;

namespace GameFramework
{
    /// <summary>
    /// 敌人攻击者接口 - 定义敌人攻击相关属性和事件
    /// </summary>
    public interface IEnemyAttacker
    {
        /// <summary>
        /// 是否正在攻击
        /// </summary>
        bool IsAttacking { get; }

        /// <summary>
        /// 攻击开始事件
        /// </summary>
        event Action OnAttackStart;

        /// <summary>
        /// 攻击结束事件
        /// </summary>
        event Action OnAttackEnd;
    }
}
