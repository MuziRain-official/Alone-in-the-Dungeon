using System;
using PlayerController;
using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    public static PlayerHealth Instance { get; private set; }
    public float maxHealth;
    public float currentHealth;
    public event Action<float> OnDamage;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        currentHealth = maxHealth;
    }


    void Update()
    {
        
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnDamage?.Invoke(damage);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(PlayerManager.Instance.gameObject);
    }
}
