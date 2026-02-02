/* WeaponAimBasic.cs
 * 基础武器瞄准脚本，控制武器朝向鼠标位置
 */

using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class WeaponController : MonoBehaviour
    {
        private Camera m_mainCamera;
        private Transform m_playerTransform;
        
        void Start()
        {
            m_mainCamera = Camera.main;
            m_playerTransform = transform.parent;
        }
        
        void Update()
        {
            // 更新武器朝向
            UpdateWeaponRotation();
            
            // 如果角色翻转了，武器也翻转来"正过来"
            transform.localScale = m_playerTransform.localScale.x < 0 
                ? new Vector3(-1, -1, 1) 
                : Vector3.one;
        }
        
        private void UpdateWeaponRotation()
        {
            if (m_mainCamera == null) return;
            
            // 获取鼠标位置
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mousePosition = m_mainCamera.ScreenToWorldPoint(
                new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, m_mainCamera.nearClipPlane));
            
            // 计算方向并旋转
            Vector2 direction = mousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}