/* MovementController.cs 
* 角色动画控制脚本
*/
using UnityEngine;

namespace PlayerController
{
    public class AnimationController : MonoBehaviour
    {
        public Animator playerAnimator;
        
        private MovementController m_movementController;
        
        void Start()
        {
            playerAnimator = GetComponent<Animator>();
            m_movementController = GetComponent<MovementController>();
        }
        
        void Update()
        {
            if (playerAnimator == null || m_movementController == null) return;
            
            Vector2 velocity = m_movementController.GetCurrentVelocity();
            bool isMoving = velocity.magnitude > 0.1f;
            
            playerAnimator.SetBool("isMoving", isMoving);

        }
        
        public void PlayAnimation(string animationName)
        {
            if (playerAnimator != null)
            {
                playerAnimator.Play(animationName);
            }
        }
    }
}