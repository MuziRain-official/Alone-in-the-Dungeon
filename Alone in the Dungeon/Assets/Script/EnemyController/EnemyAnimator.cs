using UnityEngine;

namespace EnemyController
{
    public class EnemyAnimator : MonoBehaviour
    {
        [Header("动画组件")]
        public Animator animator;
        [Header("死亡特效")]
        public GameObject deathEffect;
        
        void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            
            // 初始化动画状态
            if (animator != null)
            {
                animator.SetBool("isMoving", false);
            }
        }
        
        // 这些方法由EnemyManager调用
        public void HandleMovementChanged(bool isMoving)
        {
            if (animator != null)
            {
                animator.SetBool("isMoving", isMoving);
            }
        }
        
        public void HandleHurt()
        {
            if (animator != null)
            {
                animator.SetTrigger("Hurt");
            }
        }
        
        public void HandleDie()
        {
            if (animator != null)
            {
                animator.SetBool("isMoving", false);
            }
            
            PlayDeathEffect();
        }
        
        private void PlayDeathEffect()
        {
            if (deathEffect != null)
            {
                GameObject effect = Instantiate(deathEffect, transform.position, transform.rotation);
                Destroy(effect, 2f);
            }
        }
    }
}