using System;
using PlayerController;
using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    public static PlayerHealth Instance { get; private set; }
    public float maxHealth;
    public float currentHealth;
    public event Action<float> OnDamage;
    public event Action OnDeath;
    public event Action<float> OnHeal;
    private DashSkill dashSkill;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        dashSkill = GetComponent<DashSkill>();
        currentHealth = maxHealth;
    }


    void Update()
    {
        
    }
    public void TakeDamage(int damage)
    {
        if(dashSkill.IsInvincible)
        {
            return; // 如果无敌，则不受到伤害
        }
        currentHealth -= damage;
        AudioManager.instance.PlaySFX(1);
        OnDamage?.Invoke(damage);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        OnDeath?.Invoke();
        Destroy(PlayerManager.Instance.gameObject);
    }
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        AudioManager.instance.PlaySFX(2);
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHeal?.Invoke(healAmount);
    }
}
