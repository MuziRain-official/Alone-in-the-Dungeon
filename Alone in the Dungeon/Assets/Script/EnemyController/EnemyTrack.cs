using UnityEngine;
using PlayerController;

namespace EnemyController
{
    public class EnemyTrack : MonoBehaviour
    {
        [Header("敌人移动速度")]
        public float moveSpeed = 3f;
        [Header("追踪更新间隔")]
        public float updateInterval = 0.5f;
        [Header("追踪范围")]
        public float trackingRange = 5f;
        
        private Rigidbody2D rb;
        private Transform playerTransform;
        private float nextUpdateTime;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.linearDamping = 5f;
            
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
        }
        
        private void FixedUpdate()
        {
            if (playerTransform != null)
            {
                if(Vector2.Distance(playerTransform.position, transform.position) <= trackingRange)
                {
                    Vector2 direction = (playerTransform.position - transform.position).normalized;
                    rb.linearVelocity = direction * moveSpeed;
                }
            }
            else
            {
                // 如果没有找到玩家，停止移动
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}