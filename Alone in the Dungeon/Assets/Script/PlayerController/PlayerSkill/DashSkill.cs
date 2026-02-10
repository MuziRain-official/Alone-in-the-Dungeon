using System;
using UnityEngine;
using UnityEngine.InputSystem;
using EnemyController;

namespace PlayerController
{
    public class DashSkill : MonoBehaviour
    {
        [Header("冲刺速度加成")]
        public float dashSpeedBoost = 15f;
        [Header("冲刺持续时间")]
        public float dashDuration = 0.2f;
        [Header("冷却时间")]
        public float dashCooldown = 1f;
        [Header("冲刺伤害")]
        public int dashDamage = 5;
        
        public event Action OnDashStart;
        
        private float dashTimer;
        private float cooldownTimer;
        private Rigidbody2D m_rb;
        private BoxCollider2D m_collider;
        private Vector2 originalVelocity;
        private GameObject weaponObject;
        
        void Start()
        {
            m_rb = GetComponent<Rigidbody2D>();
            m_collider = GetComponent<BoxCollider2D>();
            weaponObject = transform.Find("Weapon")?.gameObject;
        }
        
        void Update()
        {
            // 冲刺计时
            if (dashTimer > 0)
            {
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    // 冲刺结束，恢复原始速度和碰撞状态
                    m_rb.linearVelocity = originalVelocity;
                    EndDash();
                }
            }
            
            // 冷却计时
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }
        }
        
        public bool IsDashing()
        {
            return dashTimer > 0;
        }
        
        private void StartDash()
        {
            // 禁用武器
            if (weaponObject != null)
                weaponObject.SetActive(false);
            
            // 开启触发器模式，不与敌人物理碰撞
            if (m_collider != null)
                m_collider.isTrigger = true;
        }
        
        private void EndDash()
        {
            // 启用武器
            if (weaponObject != null)
                weaponObject.SetActive(true);
            
            // 关闭触发器模式，恢复与敌人的物理碰撞
            if (m_collider != null)
                m_collider.isTrigger = false;
        }
        
        // 与敌人的触发器碰撞检测
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsDashing() && other.CompareTag("Enemy"))
            {
                // 调用敌人的受伤方法
                EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(dashDamage);
                }
            }
        }
                
        public void Dash(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                // 检查是否可以冲刺
                if (cooldownTimer <= 0 && dashTimer <= 0)
                {            
                    // 保存当前速度
                    originalVelocity = m_rb.linearVelocity;
                    
                    // 在当前速度基础上增加爆发速度
                    Vector2 boostDirection = originalVelocity.normalized;
                    if (boostDirection.magnitude < 0.1f)
                    {
                        // 如果当前几乎静止，使用默认方向（比如向右）
                        boostDirection = Vector2.right;
                    }
                    
                    // 施加爆发速度
                    m_rb.linearVelocity += boostDirection * dashSpeedBoost;
                    // 触发冲刺事件
                    OnDashStart?.Invoke();
                    
                    // 启动冲刺状态
                    StartDash();
                    
                    // 重置计时器
                    dashTimer = dashDuration;
                    cooldownTimer = dashCooldown;
                }
            }
        }
    }
}