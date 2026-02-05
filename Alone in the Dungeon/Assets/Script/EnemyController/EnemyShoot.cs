using UnityEngine;
using System.Collections;
using PlayerController;

namespace EnemyController
{
    public class EnemyShoot : MonoBehaviour, IEnemyAttacker
    {
        [Header("射击设置")]
        public float attackRange = 5f;
        public float attackCooldown = 2f;
        public float shootingDuration = 1f; // 射击持续时间
        
        // 实现接口中的事件
        public event System.Action OnAttackStart;
        public event System.Action OnAttackEnd;
        
        private Transform playerTransform;
        private Rigidbody2D rb;
        private bool isAttacking;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>(); // 获取Rigidbody2D
            
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
            OnAttackStart?.Invoke(); // 触发攻击开始事件
            
            // 执行射击行为
            StartCoroutine(Shoot());
            
            // 开始冷却
            StartCoroutine(AttackCooldown());
        }
        
        private IEnumerator Shoot()
        {
            // 射击前停止移动
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            
            // 射击逻辑 - 在这里添加具体的射击代码
            // 例如：发射子弹、播放射击动画等
            
            Debug.Log("敌人开始射击");
            
            // 等待射击持续时间
            yield return new WaitForSeconds(shootingDuration);
            
            // 结束射击
            Debug.Log("敌人结束射击");
            isAttacking = false;
            OnAttackEnd?.Invoke(); // 触发攻击结束事件
        }
        
        private IEnumerator AttackCooldown()
        {
            // 禁用攻击检测
            enabled = false;
            yield return new WaitForSeconds(attackCooldown);
            enabled = true;
        }
        
        // 接口实现
        public bool IsAttacking => isAttacking;
    }
}