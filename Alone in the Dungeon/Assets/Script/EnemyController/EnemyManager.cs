using UnityEngine;

namespace EnemyController
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("敌人组件")]
        public EnemyMovement movement;
        public EnemyHealth health;
        public EnemyAnimator animator;
        
        void Start()
        {
            // 确保所有组件都已获取
            InitializeComponents();
            
            // 设置组件间的连接
            SetupComponentConnections();
        }
        
        private void InitializeComponents()
        {
            if (movement == null)
                movement = GetComponent<EnemyMovement>();
            if (health == null)
                health = GetComponent<EnemyHealth>();
            if (animator == null)
                animator = GetComponent<EnemyAnimator>();
        }
        
        private void SetupComponentConnections()
        {
            // 连接移动组件事件到动画组件
            if (movement != null && animator != null)
            {
                movement.OnMovementChanged += animator.HandleMovementChanged;
            }
            
            // 连接生命组件事件到动画组件
            if (health != null && animator != null)
            {
                health.OnHurt += animator.HandleHurt;
                health.OnDie += animator.HandleDie;
            }
        }
        
        void OnDestroy()
        {
            // 清理事件订阅
            if (movement != null && animator != null)
            {
                movement.OnMovementChanged -= animator.HandleMovementChanged;
            }
            
            if (health != null && animator != null)
            {
                health.OnHurt -= animator.HandleHurt;
                health.OnDie -= animator.HandleDie;
            }
        }
    }
}