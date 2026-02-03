/* MovementController.cs 
* 角色移动控制脚本
*/
using UnityEngine;

namespace PlayerController
{
    public class MovementController : MonoBehaviour
    {
        [Header("移动设置")]
        public float moveSpeed = 5f;
        
        private Rigidbody2D m_rb;
        private InputHandler m_inputHandler;
        
        void Start()
        {
            m_rb = GetComponent<Rigidbody2D>();
            m_inputHandler = GetComponent<InputHandler>();
        }
        
        void FixedUpdate()//物理更新
        {
            if (m_inputHandler == null) return;
            
            Vector2 input = m_inputHandler.MoveInput;
            
            if (input.magnitude > 1f)
            {
                input = input.normalized;
            }
            
            Vector2 movement = input * moveSpeed;
            m_rb.linearVelocity = movement;
        }
        
        public Vector2 GetCurrentVelocity()//获取当前速度
        {
            return m_rb != null ? m_rb.linearVelocity : Vector2.zero;
        }
    }
}