/* PlayerLogic_Animator.cs
 * 玩家逻辑脚本，包含移动、朝向和动画控制
 */

using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class PlayerLogic_Animator : MonoBehaviour
    {
        [Header("移动设置")]
        public float moveSpeed = 5f;
        public bool normalizeInput = true;
        
        [Header("朝向设置")]
        public bool faceMouse = true;
        
        [Header("动画设置")]
        public Animator playerAnimator;
        private Vector2 m_currentMovementInput;
        private Rigidbody2D m_rb;
        private Camera m_mainCamera;
        
        void Start()
        {
            m_rb = GetComponent<Rigidbody2D>();
            m_mainCamera = Camera.main;
            playerAnimator = GetComponent<Animator>();
        }
        void Update()
        {
            if (faceMouse && m_mainCamera != null)
            {
                FaceMouseDirection();
            }
            
            UpdateAnimationParameters();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            m_currentMovementInput = context.ReadValue<Vector2>();
        }

        void FixedUpdate()
        {
            Vector2 input = m_currentMovementInput;
            
            if (normalizeInput && input.magnitude > 1f)
            {
                input = input.normalized;
            }
            
            Vector2 movement = input * moveSpeed;
            m_rb.linearVelocity = movement;
        }
        
        private void FaceMouseDirection()
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mousePosition = m_mainCamera.ScreenToWorldPoint(
                new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, m_mainCamera.nearClipPlane));
            
            Vector3 relativeMousePosition = mousePosition - transform.position;
            
            if (relativeMousePosition.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (relativeMousePosition.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        
       private void UpdateAnimationParameters()
        {
            if (playerAnimator == null) return;

            // 判断玩家是否正在移动
            bool isMoving = m_currentMovementInput.magnitude > 0;
            
            playerAnimator.SetBool("isMoving", isMoving);
        }
    }
}