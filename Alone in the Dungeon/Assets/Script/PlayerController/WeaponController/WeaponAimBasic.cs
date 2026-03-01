/* WeaponAimBasic.cs
 * 基础武器瞄准脚本
 * 支持两种模式：
 * 1. 作为玩家子对象时，通过 transform.parent 自动获取玩家引用
 * 2. 独立存在时，通过 SetPlayerTransform 显式设置玩家引用
 */

using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class WeaponAimBasic : MonoBehaviour
    {
        private Camera m_mainCamera;
        private Transform m_playerTransform;
        private bool m_useExplicitPlayerRef = false;  // 是否使用显式设置的玩家引用

        void Start()
        {
            m_mainCamera = Camera.main;

            // 如果没有显式设置玩家引用，则尝试从父对象获取（保持向后兼容）
            if (!m_useExplicitPlayerRef)
            {
                m_playerTransform = transform.parent;
            }
        }

        /// <summary>
        /// 显式设置玩家变换引用（供 WeaponManager 调用）
        /// </summary>
        public void SetPlayerTransform(Transform playerTransform)
        {
            m_playerTransform = playerTransform;
            m_useExplicitPlayerRef = playerTransform != null;
        }

        void Update()
        {
            // 更新武器朝向
            UpdateWeaponRotation();

            // 如果有玩家引用，根据玩家朝向调整武器翻转
            if (m_playerTransform != null)
            {
                transform.localScale = m_playerTransform.localScale.x < 0
                    ? new Vector3(-1, -1, 1)
                    : Vector3.one;
            }
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