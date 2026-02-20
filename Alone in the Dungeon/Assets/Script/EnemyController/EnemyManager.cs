using UnityEngine;
using GameFramework;

namespace EnemyController
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("敌人组件")]
        public EnemyMovement movement;
        public EnemyHealth health;
        public EnemyAnimator animator;

        private IEnemyAttacker attacker;

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

            attacker = GetComponent<IEnemyAttacker>();
        }

        private void SetupComponentConnections()
        {
            if (movement != null && animator != null)
            {
                movement.OnMovementChanged += animator.HandleMovementChanged;
            }

            if (health != null && animator != null)
            {
                health.OnHurt += animator.HandleHurt;
                health.OnDied += animator.HandleDie;
            }

            if (attacker != null && animator != null)
            {
                attacker.OnAttackStart += animator.HandleAttackStart;
            }
        }

        void OnDestroy()
        {
            if (movement != null && animator != null)
            {
                movement.OnMovementChanged -= animator.HandleMovementChanged;
            }

            if (health != null && animator != null)
            {
                health.OnHurt -= animator.HandleHurt;
                health.OnDied -= animator.HandleDie;
            }

            if (attacker != null && animator != null)
            {
                attacker.OnAttackStart -= animator.HandleAttackStart;
            }
        }
    }
}
