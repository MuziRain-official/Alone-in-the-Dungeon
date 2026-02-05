using UnityEngine;
using System.Collections;
using PlayerController;

namespace EnemyController
{
    public class EnemyAttack : MonoBehaviour, IEnemyAttacker // 实现接口
    {
        [Header("攻击设置")]
        public float attackRange = 2f;
        public float attackCooldown = 2f;
        public float chargeSpeed = 8f;
        public float contactDamageInterval = 0.5f; // 接触伤害间隔
        // 攻击事件
        public event System.Action OnAttackStart;
        public event System.Action OnAttackEnd;
        
        private Transform playerTransform;
        private Rigidbody2D rb;
        private bool isAttacking;
        private float lastDamageTime; // 上次造成伤害的时间
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            
            if (PlayerManager.Instance != null)
            {
                playerTransform = PlayerManager.Instance.PlayerTransform;
            }
        }
        
        void Update()
        {            
            // 检测攻击条件
            if (!isAttacking && playerTransform != null)
            {
                float distance = Vector2.Distance(playerTransform.position, transform.position);
                if (distance <= attackRange) StartAttack();
            }
        }
        
        private void StartAttack()
        {
            isAttacking = true;
            OnAttackStart?.Invoke();
            
            // 计算冲撞方向
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            
            // 开始冲撞
            StartCoroutine(ChargeAttack(direction));
            
            // 开始冷却
            StartCoroutine(AttackCooldown());
        }
        
        private IEnumerator ChargeAttack(Vector2 direction)
        {
            // 冲刺一小段距离
            float chargeTime = 0.3f;
            rb.linearVelocity = direction * chargeSpeed;
            
            yield return new WaitForSeconds(chargeTime);
            
            // 停止冲撞
            rb.linearVelocity = Vector2.zero;
            isAttacking = false;
            OnAttackEnd?.Invoke();
        }
        
        private IEnumerator AttackCooldown()
        {
            // 禁用攻击检测
            enabled = false;
            yield return new WaitForSeconds(attackCooldown);
            enabled = true;
        }
        
        // 碰撞检测（攻击玩家）
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isAttacking && collision.gameObject.CompareTag("Player"))
            {
                DealDamage(collision.gameObject);
                
                // 立即停止冲撞
                StopCoroutine(nameof(ChargeAttack));
                rb.linearVelocity = Vector2.zero;
                isAttacking = false;
                OnAttackEnd?.Invoke();
            }
        }
        
        // 持续碰撞检测（当敌人和玩家保持接触时）
        private void OnCollisionStay2D(Collision2D collision)
        {
            // 只有当敌人处于攻击状态时才造成伤害
            if (isAttacking && collision.gameObject.CompareTag("Player"))
            {
                DealDamage(collision.gameObject);
            }
        }
        
        // 伤害处理方法
        private void DealDamage(GameObject player)
        {
            // 检查伤害间隔，避免每帧都造成伤害
            if (Time.time - lastDamageTime >= contactDamageInterval)
            {
                if (PlayerHealth.Instance != null)
                {
                    PlayerHealth.Instance.TakeDamage(10f);
                }
                lastDamageTime = Time.time;
            }
        }
        
        // 接口实现
        public bool IsAttacking => isAttacking;
    }
}