using UnityEngine;
using System.Collections;
using PlayerController;

namespace EnemyController
{
    public class EnemyAttack : MonoBehaviour
    {
        [Header("攻击设置")]
        public float attackRange = 2f;
        public float attackCooldown = 2f;
        public float chargeSpeed = 8f;
        
        // 攻击事件
        public event System.Action OnAttackStart;
        public event System.Action OnAttackEnd;
        
        private Transform playerTransform;
        private Rigidbody2D rb;
        private bool isAttacking;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            
            // 使用PlayerManager单例获取玩家Transform
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
        // 取消勾选敌人的"Is Trigger"，使用碰撞器而不是触发器

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isAttacking && collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Enemy attacked the player!");
                
                // 立即停止冲撞
                StopCoroutine(nameof(ChargeAttack));
                rb.linearVelocity = Vector2.zero;
                isAttacking = false;
                OnAttackEnd?.Invoke();
            }
        }
        
        // 属性
        public bool IsAttacking => isAttacking;
    }
}