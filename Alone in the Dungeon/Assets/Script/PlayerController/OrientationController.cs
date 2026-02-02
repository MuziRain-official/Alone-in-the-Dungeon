/* MovementController.cs 
* 角色朝向控制脚本
*/
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class OrientationController : MonoBehaviour
    {
        [Header("朝向设置")]
        public bool faceMouse = true;
        
        private Camera m_mainCamera;
        private InputHandler m_inputHandler;
        
        public bool IsFacingRight { get; private set; } = true;
        public System.Action<bool> OnDirectionChanged; // 事件：方向改变时触发
        
        void Start()
        {
            m_mainCamera = Camera.main;
            m_inputHandler = GetComponent<InputHandler>();
        }
        
        void Update()
        {
            if (faceMouse && m_inputHandler != null && m_mainCamera != null)
            {
                FaceMouseDirection();
            }
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
        
        public void SetDirection(bool faceRight)
        {
            if (faceRight != IsFacingRight)
            {
                IsFacingRight = faceRight;
                transform.localScale = new Vector3(IsFacingRight ? 1 : -1, 1, 1);
                OnDirectionChanged?.Invoke(IsFacingRight);
            }
        }
    }
}