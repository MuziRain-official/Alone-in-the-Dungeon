using UnityEngine;
using PlayerController;

namespace EnemyController
{
    public class EnemyTrack : MonoBehaviour
    {
        [Header("敌人生命值")]
        public int health = 20;
        [Header("敌人移动速度")]
        public float moveSpeed = 3f;
        [Header("追踪更新间隔")]
        public float updateInterval = 0.5f;
        [Header("追踪范围")]
        public float trackingRange = 5f;
        [Header("敌人动画组件")]
        public Animator animator;
        [Header("敌人死亡特效")]
        public GameObject deathEffect;
        private bool isPlayerInRange = false;
        
        private Rigidbody2D rb;
        private Transform playerTransform;
        private float nextUpdateTime;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.linearDamping = 5f;

            animator = GetComponent<Animator>();
            
            // 通过PlayerManager单例获取玩家Transform
            if (PlayerManager.Instance != null)
            {
                playerTransform = PlayerManager.Instance.PlayerTransform;
            }
            
            nextUpdateTime = Time.time + updateInterval;
        }
        
        void Update()
        {
            // 定期重新获取玩家位置
            if (Time.time >= nextUpdateTime)
            {
                if (playerTransform == null && PlayerManager.Instance != null)
                {
                    playerTransform = PlayerManager.Instance.PlayerTransform;
                }
                nextUpdateTime = Time.time + updateInterval;
            }
            
            // 更新动画状态
            UpdateAnimation();
        }
        
        private void FixedUpdate()
        {
            if (playerTransform != null)
            {
                float distance = Vector2.Distance(playerTransform.position, transform.position);
                
                if(distance <= trackingRange)
                {
                    Vector2 direction = (playerTransform.position - transform.position).normalized;
                    rb.linearVelocity = direction * moveSpeed;
                    isPlayerInRange = true;
                }
                else
                {
                    // 玩家超出范围，停止移动
                    rb.linearVelocity = Vector2.zero;
                    isPlayerInRange = false;
                }
            }
            else
            {
                // 如果没有找到玩家，停止移动
                rb.linearVelocity = Vector2.zero;
                isPlayerInRange = false;
            }
        }

        void UpdateAnimation()
        {
            if (animator != null)
            {
                // 只有当玩家在范围内且正在移动时才播放移动动画
                bool shouldMove = isPlayerInRange && playerTransform != null;
                animator.SetBool("isMoving", shouldMove);
            }
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            animator.SetTrigger("Hurt");
            if (health <= 0)
            {
                Destroy(gameObject);
                Instantiate(deathEffect, transform.position, transform.rotation);
            }
        }
    }
}