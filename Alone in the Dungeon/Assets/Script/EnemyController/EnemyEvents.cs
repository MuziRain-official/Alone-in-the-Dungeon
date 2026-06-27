namespace EnemyController
{
    /// <summary>
    /// 敌人死亡事件
    /// </summary>
    public struct EnemyDeathEvent
    {
        public UnityEngine.GameObject enemy;
    }

    /// <summary>
    /// Boss激活事件
    /// </summary>
    public struct BossActivationEvent
    {
        public UnityEngine.GameObject boss;
        public EnemyHealth bossHealth;
    }

    /// <summary>
    /// Boss受伤事件
    /// </summary>
    public struct BossDamageEvent
    {
        public UnityEngine.GameObject boss;
        public float currentHealth;
        public float maxHealth;
    }
}
