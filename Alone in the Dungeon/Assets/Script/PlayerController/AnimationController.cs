/* AnimationController.cs 
* 角色动画控制脚本
*/
using UnityEngine;

namespace PlayerController
{
    public class AnimationController : MonoBehaviour
    {
        public Animator playerAnimator;
        public PlayerHealth playerHealth;
        
        private MovementController m_movementController; 
        
        void Start()
        {
            playerAnimator = GetComponent<Animator>();
            m_movementController = GetComponent<MovementController>();

            playerHealth = PlayerHealth.Instance;
            if (playerHealth != null)
            {
                playerHealth.OnDamage += HandleDamage;
            }
        }
        
        void Update()
        {
            if (playerAnimator == null || m_movementController == null) return;
            
            Vector2 velocity = m_movementController.GetCurrentVelocity();//获取当前速度
            bool isMoving = velocity.magnitude > 0.1f;
            
            playerAnimator.SetBool("isMoving", isMoving);

        }
        
        private void HandleDamage(float damage)
        {
            playerAnimator.SetTrigger("isHurt");
        }
    }
}