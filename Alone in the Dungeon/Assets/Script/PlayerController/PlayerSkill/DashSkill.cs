using System;
using UnityEngine;
using UnityEngine.InputSystem;
using EnemyController;

namespace PlayerController
{
    public class DashSkill : MonoBehaviour
    {
        [Header("冲刺速度")]
        public float dashSpeedBoost = 15f;
        [Header("冲刺持续时间")]
        public float dashDuration = 0.2f;
        [Header("冷却时间")]
        public float dashCooldown = 1f;
        [Header("冲刺伤害")]
        public int dashDamage = 5;
        
        public event Action OnDashStart;

        [Header("冲刺时无敌")]
        public bool IsInvincible { get; private set; }
        
        private float dashTimer;
        private float cooldownTimer;
        private Rigidbody2D m_rb;
        private BoxCollider2D m_collider;
        private Vector2 originalVelocity;
        private GameObject weaponObject;
        private Vector2 dashDirection;
        
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

        void FixedUpdate()
        {
            if (IsDashing())
            {
                // 保持冲刺速度方向不变，防止被物理效果减速
                m_rb.linearVelocity = dashDirection * dashSpeedBoost;
            }
        }
        
        public bool IsDashing()
        {
            return dashTimer > 0;
        }
        
        private void StartDash()
        {
            IsInvincible = true;
            
            // 禁用武器
            if (weaponObject != null)
                weaponObject.SetActive(false);
            
            // 开启触发器模式，不与敌人物理碰撞
            if (m_collider != null)
                m_collider.isTrigger = true;
        }
        
        private void EndDash()
        {
            IsInvincible = false;
            
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
                if (cooldownTimer <= 0 && dashTimer <= 0)
                {
                    originalVelocity = m_rb.linearVelocity;

                    // 正确计算鼠标世界坐标：使用玩家到相机的距离作为 Z 深度
                    Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
                    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(
                        mouseScreenPos.x,
                        mouseScreenPos.y,
                        transform.position.z - Camera.main.transform.position.z
                    ));

                    dashDirection = (mouseWorldPos - transform.position).normalized;

                    if (dashDirection.magnitude < 0.1f)
                        dashDirection = transform.right;

                    m_rb.linearVelocity = dashDirection * dashSpeedBoost;

                    OnDashStart?.Invoke();
                    StartDash();
                    dashTimer = dashDuration;
                    cooldownTimer = dashCooldown;
                }
            }
        }
    }
}