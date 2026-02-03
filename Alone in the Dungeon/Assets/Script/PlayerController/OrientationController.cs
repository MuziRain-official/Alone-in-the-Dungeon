/* OrientationController.cs 
* 角色朝向控制脚本
*/
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class OrientationController : MonoBehaviour
    {        
        private Camera m_mainCamera;
        private InputHandler m_inputHandler;
        
        void Start()
        {
            m_mainCamera = Camera.main;
            m_inputHandler = GetComponent<InputHandler>();
        }
        
        void Update()
        {
            if (m_inputHandler != null && m_mainCamera != null)
            {
                FaceMouseDirection();
            }
        }
        
        private void FaceMouseDirection()//使角色面向鼠标方向
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
    }
}