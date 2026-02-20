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
        [Range(0f, 1f)]
        public float sprintMultiplier = 0.3f; // 按住shift时速度增加百分比
        
        private Rigidbody2D m_rb;
        private InputHandler m_inputHandler;
        private DashSkill m_dashSkill;
        
        void Start()
        {
            m_rb = GetComponent<Rigidbody2D>();
            m_inputHandler = GetComponent<InputHandler>();
            m_dashSkill = GetComponent<DashSkill>();
        }
        
        void FixedUpdate()//物理更新
        {
            // 如果有冲刺组件且正在冲刺，跳过移动更新
            if (m_dashSkill != null && m_dashSkill.IsDashing())
            {
                return;
            }

            if (m_inputHandler == null) return;

            Vector2 input = m_inputHandler.MoveInput;

            if (input.magnitude > 1f)
            {
                input = input.normalized;
            }

            // 计算当前速度：如果按下shift则应用加速倍率
            float currentSpeed = moveSpeed;
            if (m_inputHandler.IsSprinting)
            {
                currentSpeed *= (1f + sprintMultiplier);
            }

            Vector2 movement = input * currentSpeed;
            m_rb.linearVelocity = movement;
        }
        
        public Vector2 GetCurrentVelocity()//获取当前速度
        {
            return m_rb != null ? m_rb.linearVelocity : Vector2.zero;
        }
    }
}