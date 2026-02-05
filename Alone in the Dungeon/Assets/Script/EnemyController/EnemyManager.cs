using UnityEngine;

namespace EnemyController
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("敌人组件")]
        public EnemyMovement movement;
        public EnemyHealth health;
        public EnemyAnimator animator;
        
        private IEnemyAttacker attacker; // 使用接口类型
        
        void Start()
        {
            InitializeComponents();
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
            
            // 获取攻击组件（通过接口）
            attacker = GetComponent<IEnemyAttacker>();
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
            
            // 连接攻击组件事件到动画组件（通过接口）
            if (attacker != null && animator != null)
            {
                attacker.OnAttackStart += animator.HandleAttackStart;
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
            
            if (attacker != null && animator != null)
            {
                attacker.OnAttackStart -= animator.HandleAttackStart;
            }
        }
    }
}