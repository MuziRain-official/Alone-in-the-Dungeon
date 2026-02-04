using UnityEngine;
using PlayerController;
using System;

namespace EnemyController
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("移动设置")]
        public float moveSpeed = 3f;
        public float trackingRange = 5f;
        
        // 移动状态事件
        public event Action<bool> OnMovementChanged;
        
        private Rigidbody2D rb;
        private Transform playerTransform;
        private bool isMoving = false;
        private EnemyAttack enemyAttack;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            enemyAttack = GetComponent<EnemyAttack>();
            
            // 获取玩家Transform
            if (PlayerManager.Instance != null)
            {
                playerTransform = PlayerManager.Instance.PlayerTransform;
            }
        }
        
        void Update()
        {
            // 如果玩家Transform为空，尝试重新获取
            if (playerTransform == null && PlayerManager.Instance != null)
            {
                playerTransform = PlayerManager.Instance.PlayerTransform;
            }
        }
        
        private void FixedUpdate()
        {
            // 如果正在攻击，不执行追踪逻辑
            if (enemyAttack != null && enemyAttack.IsAttacking)
            {
                return;
            }
            
            if (playerTransform != null)
            {
                float distance = Vector2.Distance(playerTransform.position, transform.position);
                
                if (distance <= trackingRange)
                {
                    MoveTowardsPlayer();
                }
                else
                {
                    StopMovement();
                }
            }
            else
            {
                StopMovement();
            }
        }
        
        private void MoveTowardsPlayer()
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
            
            if (!isMoving)
            {
                isMoving = true;
                OnMovementChanged?.Invoke(true);
            }
        }
        
        private void StopMovement()
        {
            rb.linearVelocity = Vector2.zero;
            
            if (isMoving)
            {
                isMoving = false;
                OnMovementChanged?.Invoke(false);
            }
        }
        
        // 供其他组件访问的属性
        public bool IsMoving => isMoving;
    }
}