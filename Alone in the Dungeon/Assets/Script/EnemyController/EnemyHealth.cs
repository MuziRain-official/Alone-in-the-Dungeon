using UnityEngine;
using System;

namespace EnemyController
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [Header("生命值设置")]
        public int maxHealth = 20;
        public int currentHealth;
        
        // 生命值事件
        public event Action OnHurt;
        public event Action OnDie;
        
        void Start()
        {
            currentHealth = maxHealth;
        }
        
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            OnHurt?.Invoke();
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        }
        
        private void Die()
        {
            OnDie?.Invoke();
            Destroy(gameObject);
        }
        
        public float GetHealthPercentage()
        {
            return (float)currentHealth / maxHealth;
        }
    }
}