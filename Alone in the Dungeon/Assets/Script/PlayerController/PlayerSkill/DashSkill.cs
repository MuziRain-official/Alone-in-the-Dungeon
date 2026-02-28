using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EnemyController;
using GameFramework;

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
        [Header("卡肉持续时间")]
        public float hitStopDuration = 0.08f;
        
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
        private bool isPaused; // 卡肉期间暂停物理更新
        private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>(); // 记录已伤害的敌人
        
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
            // 卡肉期间不更新物理
            if (isPaused) return;

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

            // 清空已伤害敌人列表
            damagedEnemies.Clear();
        }
        
        // 与敌人的触发器碰撞检测
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsDashing() && other.CompareTag("Enemy") && !damagedEnemies.Contains(other.gameObject))
            {
                // 标记该敌人已受伤
                damagedEnemies.Add(other.gameObject);

                // 优先使用EnemyHealth组件
                EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(dashDamage);
                    TriggerHitStop();
                }
                else
                {
                    // 检查IDamageable接口（Boss等使用独立血量系统的敌人）
                    IDamageable damageable = other.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(dashDamage);
                        TriggerHitStop();
                    }
                }
            }
        }

        // 卡肉效果 - 玩家短暂停顿
        private void TriggerHitStop()
        {
            StartCoroutine(HitStopCoroutine());
        }

        private System.Collections.IEnumerator HitStopCoroutine()
        {
            // 标记暂停状态，阻止FixedUpdate更新速度
            isPaused = true;
            m_rb.linearVelocity = Vector2.zero;

            float pauseTimer = hitStopDuration;
            while (pauseTimer > 0)
            {
                yield return null;
                pauseTimer -= Time.deltaTime;
                m_rb.linearVelocity = Vector2.zero;
            }

            // 恢复物理更新
            isPaused = false;
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