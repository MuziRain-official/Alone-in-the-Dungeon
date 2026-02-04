using UnityEngine;
using PlayerController;
using System;

namespace EnemyController
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("移动设置")]
        public float moveSpeed = 3f;
        public float updateInterval = 0.5f;
        public float trackingRange = 5f;
        
        // 移动状态事件
        public event Action<bool> OnMovementChanged;
        
        private Rigidbody2D rb;
        private Transform playerTransform;
        private float nextUpdateTime;
        private bool isMoving = false;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            
            if (PlayerManager.Instance != null)
            {
                playerTransform = PlayerManager.Instance.PlayerTransform;
            }
            
            nextUpdateTime = Time.time + updateInterval;
        }
        
        void Update()
        {
            // 定期更新玩家位置
            if (Time.time >= nextUpdateTime)
            {
                UpdatePlayerTransform();
                nextUpdateTime = Time.time + updateInterval;
            }
        }
        
        private void FixedUpdate()
        {
            HandleMovement();
        }
        
        private void UpdatePlayerTransform()
        {
            if (playerTransform == null && PlayerManager.Instance != null)
            {
                playerTransform = PlayerManager.Instance.PlayerTransform;
            }
        }
        
        private void HandleMovement()
        {
            if (playerTransform == null)
            {
                StopMovement();
                return;
            }
            
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
    }
}