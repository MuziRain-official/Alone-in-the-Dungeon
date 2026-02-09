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
        private IEnemyAttacker enemyAttacker;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            enemyAttacker = GetComponent<IEnemyAttacker>();
            
            if (PlayerManager.Instance != null)
            {
                playerTransform = PlayerManager.Instance.PlayerTransform;
            }
        }
        
        void Update()
        {
            if (playerTransform == null && PlayerManager.Instance != null)
            {
                playerTransform = PlayerManager.Instance.PlayerTransform;
            }
        }
        
        private void FixedUpdate()
        {
            // 如果正在攻击，不执行追踪逻辑
            if (enemyAttacker != null && enemyAttacker.IsAttacking)
            {
                return;
            }
            
            if (playerTransform != null)
            {
                float distance = Vector2.Distance(playerTransform.position, transform.position);
                
                if (distance <= trackingRange)
                {
                    MoveTowardsPlayer();
                    UpdateFacingDirection();
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
        
        private void UpdateFacingDirection()
        {
            if (playerTransform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
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
        
        public bool IsMoving => isMoving;
    }
}