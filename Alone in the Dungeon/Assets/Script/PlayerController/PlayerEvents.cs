namespace PlayerController
{
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
}
