/* PlayerLogic.cs - 使用localScale实现角色朝向 */
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class PlayerLogic : MonoBehaviour
    {
        [Header("移动设置")]
        [Tooltip("移动速度")] 
        public float moveSpeed = 5f;
        [Tooltip("是否归一化输入向量")]
        public bool normalizeInput = true;
        
        [Header("朝向设置")]
        [Tooltip("是否面向鼠标")]
        public bool faceMouse = true;

        private Vector2 m_currentMovementInput;
        private Rigidbody2D m_rb;
        private Camera m_mainCamera;
        
        void Start()
        {
            m_rb = GetComponent<Rigidbody2D>();
            m_mainCamera = Camera.main;
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
        
        void Update()
        {
            if (faceMouse && m_mainCamera != null)
            {
                FaceMouseDirection();
            }
        }
        
        /// <summary>
        /// 角色朝向鼠标方向（使用localScale实现）
        /// </summary>
        private void FaceMouseDirection()
        {
            // 获取鼠标位置
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mousePosition = m_mainCamera.ScreenToWorldPoint(
                new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, m_mainCamera.nearClipPlane));
            
            // 比较角色和鼠标的位置
            Vector3 relativeMousePosition = mousePosition - transform.position;
            
            // 如果鼠标在角色右侧，面朝右；在左侧，面朝左
            if (relativeMousePosition.x > 0)
            {
                // 面朝右（正常缩放）
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (relativeMousePosition.x < 0)
            {
                // 面朝左（水平翻转）
                transform.localScale = new Vector3(-1, 1, 1);
            }
            // 如果x坐标为0，保持当前朝向
        }
    }
}